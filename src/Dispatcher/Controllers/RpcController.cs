using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Dispatcher.Model;
using Dispatcher.Services;
using System.ComponentModel.DataAnnotations;
using Dispatcher.Model.Requests;

namespace Dispatcher.Controllers
{
    /// <summary>
    /// JSON-RPC 2.0 API controller
    /// </summary>
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

        /// <summary>
        /// Handles JSON-RPC 2.0 requests
        /// </summary>
        /// <param name="request">The JSON-RPC request</param>
        /// <returns>A JSON-RPC response</returns>
        /// <response code="200">Returns the successful response</response>
        /// <response code="400">Returns error response for invalid requests</response>
        /// <response code="500">Returns error response for internal server errors</response>
        [HttpPost]
        [Route("/rpc")]
        [ProducesResponseType(typeof(JsonRpcSuccessResponse<string>), 200)]
        [ProducesResponseType(typeof(JsonRpcErrorResponse), 400)]
        [ProducesResponseType(typeof(JsonRpcErrorResponse), 500)]
        public async Task<IActionResult> HandleRpc([FromBody] JsonRpcRequest request)
        {
            try
            {
                // Validate the request model
                var validationContext = new ValidationContext(request);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
                {
                    var error = validationResults.FirstOrDefault();
                    return BadRequest(new JsonRpcErrorResponse
                    {
                        Id = null,
                        Error = new JsonRpcError
                        {
                            Code = -32600,
                            Message = error?.ErrorMessage ?? "Invalid Request"
                        }
                    });
                }

                // Get the method and id from the request
                if (request == null || string.IsNullOrEmpty(request.Method.ToString()))
                {
                    return BadRequest(new JsonRpcErrorResponse
                    {
                        Id = null,
                        Error = new JsonRpcError
                        {
                            Code = -32600,
                            Message = "Invalid Request"
                        }
                    });
                }

                string methodStr = request.Method.ToString();
                string id = request.Id;

                // Try to parse the method string to enum
                if (!Enum.TryParse<JsonRpcMethod>(methodStr, true, out var method))
                {
                    return CreateMethodNotFoundResponse(id);
                }

                // Log the incoming request
                _logger.LogInformation($"RPC request received: {method} with id {id}");

                // Dynamic method routing based on the method enum
                switch (method)
                {
                    case JsonRpcMethod.Ping:
                        var pingService = _serviceProvider.GetService(typeof(IPingService)) as IPingService;
                        if (pingService == null)
                        {
                            _logger.LogError("Failed to resolve IPingService");
                            return CreateInternalErrorResponse(id);
                        }
                        try
                        {
                            var pingParams = request.GetPingParams();
                            return await pingService.HandlePing(pingParams, id);
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Error parsing ping parameters");
                            return BadRequest(new JsonRpcErrorResponse
                            {
                                Id = id,
                                Error = new JsonRpcError
                                {
                                    Code = -32602,
                                    Message = "Invalid params"
                                }
                            });
                        }
                    
                    // You can add more method cases here
                    
                    default:
                        return CreateMethodNotFoundResponse(id);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing JSON-RPC request");
                return BadRequest(new JsonRpcErrorResponse
                {
                    Id = null,
                    Error = new JsonRpcError
                    {
                        Code = -32700,
                        Message = "Parse error"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RPC request");
                return CreateInternalErrorResponse(null);
            }
        }

        private IActionResult CreateMethodNotFoundResponse(string id)
        {
            return BadRequest(new JsonRpcErrorResponse
            {
                Id = id,
                Error = new JsonRpcError
                {
                    Code = -32601,
                    Message = "Method not found"
                }
            });
        }

        private IActionResult CreateInternalErrorResponse(string id)
        {
            return StatusCode(500, new JsonRpcErrorResponse
            {
                Id = id,
                Error = new JsonRpcError
                {
                    Code = -32603,
                    Message = "Internal error"
                }
            });
        }
    }
}