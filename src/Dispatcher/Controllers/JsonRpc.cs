using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Dispatcher.Model;
using Dispatcher.Services;

namespace Dispatcher.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<IActionResult> HandleRpc([FromBody] JsonElement requestElement)
        {
            try
            {
                // Get the method and id from the request
                if (!requestElement.TryGetProperty("method", out var methodElement) || 
                    !requestElement.TryGetProperty("id", out var idElement))
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

                string method = methodElement.GetString();
                string id = idElement.GetString();

                // Log the incoming request
                _logger.LogInformation($"RPC request received: {method} with id {id}");

                // Dynamic method routing based on the method name
                switch (method)
                {
                    case "ping":
                        var pingService = _serviceProvider.GetService(typeof(IPingService)) as IPingService;
                        if (pingService == null)
                        {
                            _logger.LogError("Failed to resolve IPingService");
                            return CreateInternalErrorResponse(id);
                        }
                        return await pingService.HandlePing(requestElement, id);
                    
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