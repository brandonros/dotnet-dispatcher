using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Common.Model;
using Dispatcher.Services;
using System.ComponentModel.DataAnnotations;
using Common.Model.Requests;
using Common.Model.Responses;

namespace Dispatcher.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class RpcController : ControllerBase
{
    private readonly ILogger<RpcController> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<JsonRpcMethod, Func<JsonRpcRequestBase, string, Task<IActionResult>>> _handlers;

    public RpcController(
        ILogger<RpcController> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _handlers = new()
        {
            { JsonRpcMethod.GetUser, HandleGetUserRequest }
        };
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
            if (!ValidateRequest(request, out var errorResponse))
            {
                return errorResponse;
            }

            string methodStr = request.Method.ToString();
            string id = request.Id;

            _logger.LogInformation($"RPC request received: {methodStr} with id {id}");

            if (!_handlers.TryGetValue(request.Method, out var handler))
            {
                return CreateMethodNotFoundResponse(id);
            }

            return await handler(request, id);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing JSON-RPC request");
            return CreateParseErrorResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RPC request");
            return CreateInternalErrorResponse(null);
        }
    }

    private async Task<IActionResult> HandleGetUserRequest(JsonRpcRequestBase request, string id)
    {
        var queueService = _serviceProvider.GetService<IQueueService<GetUserRequest, GetUserResponse>>();
        if (queueService == null)
        {
            _logger.LogError("Failed to resolve IQueueService<GetUserRequest, GetUserResponse>");
            return CreateInternalErrorResponse(id);
        }

        try
        {
            if (request is not GetUserJsonRpcRequest getUserRequest)
            {
                return CreateInvalidParamsResponse(id);
            }
            var response = await queueService.RequestResponse(getUserRequest.Params);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling get user request");
            return CreateInvalidParamsResponse(id);
        }
    }

    private bool ValidateRequest(JsonRpcRequestBase request, out IActionResult errorResponse)
    {
        // Validate the request model
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var error = validationResults.FirstOrDefault();
            errorResponse = CreateInvalidRequestResponse(error?.ErrorMessage);
            return false;
        }

        if (request == null || string.IsNullOrEmpty(request.Method.ToString()))
        {
            errorResponse = CreateInvalidRequestResponse("Invalid Request");
            return false;
        }

        errorResponse = null;
        return true;
    }

    private IActionResult CreateInvalidRequestResponse(string message) =>
        BadRequest(new JsonRpcErrorResponse
        {
            Id = null,
            Error = new JsonRpcError
            {
                Code = -32600,
                Message = message ?? "Invalid Request"
            }
        });

    private IActionResult CreateMethodNotFoundResponse(string id) =>
        BadRequest(new JsonRpcErrorResponse
        {
            Id = id,
            Error = new JsonRpcError
            {
                Code = -32601,
                Message = "Method not found"
            }
        });

    private IActionResult CreateInvalidParamsResponse(string id) =>
        BadRequest(new JsonRpcErrorResponse
        {
            Id = id,
            Error = new JsonRpcError
            {
                Code = -32602,
                Message = "Invalid params"
            }
        });

    private IActionResult CreateParseErrorResponse() =>
        BadRequest(new JsonRpcErrorResponse
        {
            Id = null,
            Error = new JsonRpcError
            {
                Code = -32700,
                Message = "Parse error"
            }
        });

    private IActionResult CreateInternalErrorResponse(string id) =>
        StatusCode(500, new JsonRpcErrorResponse
        {
            Id = id,
            Error = new JsonRpcError
            {
                Code = -32603,
                Message = "Internal error"
            }
        });
}
