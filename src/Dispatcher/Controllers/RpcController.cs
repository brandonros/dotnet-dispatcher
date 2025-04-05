using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Dispatcher.Services;
using System.ComponentModel.DataAnnotations;
using Common.Model.Requests;
using Common.Model.Responses;
using MassTransit;
using Common.Model.JsonRpc;


namespace Dispatcher.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class RpcController : ControllerBase
{
    private readonly ILogger<RpcController> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RpcController(
        ILogger<RpcController> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [HttpPost]
    [Route("/rpc")]
    [ProducesResponseType(typeof(JsonRpcSuccessResponse<object>), 200)]
    [ProducesResponseType(typeof(JsonRpcErrorResponse), 400)]
    [ProducesResponseType(typeof(JsonRpcErrorResponse), 500)]
    public async Task<IActionResult> HandleRpc([FromBody] JsonRpcRequestBase request)
    {
        try
        {
            if (request == null)
            {
                return CreateErrorResponse(null, JsonRpcErrorCodes.InvalidRequest, "Invalid request");
            }

            if (!ValidateRequest(request, out var errorResponse))
            {
                return errorResponse;
            }

            string methodStr = request.Method.ToString();
            string id = request.Id;

            _logger.LogInformation($"RPC request received: {methodStr} with id {id}");

            // Dispatch to appropriate handler based on method
            return methodStr switch
            {
                "user.get" => await HandleTypedRequest<GetUserJsonRpcRequest, GetUserRequest, GetUserResponse>(request, id),
                "account.get" => await HandleTypedRequest<GetAccountJsonRpcRequest, GetAccountRequest, GetAccountResponse>(request, id),
                _ => CreateErrorResponse(id, JsonRpcErrorCodes.MethodNotFound, "Method not found")
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing JSON-RPC request");
            return CreateErrorResponse(null, JsonRpcErrorCodes.ParseError, "Parse error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RPC request");
            return CreateErrorResponse(null, JsonRpcErrorCodes.InternalError, "Internal error");
        }
    }

    private async Task<IActionResult> HandleTypedRequest<TJsonRpcRequest, TRequest, TResponse>(JsonRpcRequestBase request, string id)
        where TJsonRpcRequest : JsonRpcRequest<TRequest>
        where TRequest : IJsonRpcParams
        where TResponse : class
    {
        _logger.LogInformation($"Starting HandleTypedRequest for ID: {id}, Type: {typeof(TRequest).Name}");
        
        var queueService = _serviceProvider.GetService<IQueueService<JsonRpcRequest<TRequest>, JsonRpcSuccessResponse<TResponse>>>();
        if (queueService == null)
        {
            _logger.LogError($"Failed to resolve IQueueService for {typeof(TRequest).Name} and {typeof(TResponse).Name}");
            return CreateErrorResponse(id, JsonRpcErrorCodes.InternalError, "Internal error");
        }

        if (request is not TJsonRpcRequest typedRequest)
        {
            _logger.LogError($"Request is not compatible with {typeof(TJsonRpcRequest).Name}");
            return CreateErrorResponse(id, JsonRpcErrorCodes.InvalidParams, "Invalid params");
        }
        
        try 
        {
            _logger.LogInformation($"Sending request to queue service. RequestId: {id}");
            
            var response = await queueService.RequestResponse(typedRequest);
            
            _logger.LogInformation($"Received response from queue service for RequestId: {id}");
            
            // Return the success response directly
            return Ok(response);
        }
        catch (RequestTimeoutException ex)
        {
            _logger.LogError(ex, $"Request timed out for ID: {id}");
            return CreateErrorResponse(id, JsonRpcErrorCodes.ServerError, "Request timed out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling {typeof(TRequest).Name} request: {ex.Message}");
            return CreateErrorResponse(id, JsonRpcErrorCodes.InternalError, "Internal error");
        }
    }


    private bool ValidateRequest(JsonRpcRequestBase request, out IActionResult errorResponse)
    {
        // Check for null request or method
        if (request == null || string.IsNullOrEmpty(request.Method?.ToString()))
        {
            errorResponse = CreateErrorResponse(null, JsonRpcErrorCodes.InvalidRequest, "Invalid Request");
            return false;
        }

        // Validate using data annotations
        var validationContext = new ValidationContext(request);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var error = validationResults.FirstOrDefault();
            errorResponse = CreateErrorResponse(request.Id, JsonRpcErrorCodes.InvalidRequest, error?.ErrorMessage ?? "Invalid Request");
            return false;
        }

        errorResponse = null;
        return true;
    }

    private IActionResult CreateErrorResponse(string id, int code, string message)
    {
        var response = new JsonRpcErrorResponse
        {
            Id = id,
            Error = new JsonRpcError
            {
                Code = code,
                Message = message
            }
        };

        return code switch
        {
            JsonRpcErrorCodes.InternalError => StatusCode(500, response),
            _ => BadRequest(response)
        };
    }
}

// Define standard JSON-RPC error codes in a separate class for clarity
public static class JsonRpcErrorCodes
{
    public const int ParseError = -32700;
    public const int InvalidRequest = -32600;
    public const int MethodNotFound = -32601;
    public const int InvalidParams = -32602;
    public const int InternalError = -32603;
    public const int ServerError = -32000; // Used for timeouts and other server-specific errors
}