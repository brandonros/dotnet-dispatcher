using Microsoft.AspNetCore.Mvc;
using Common.Model.Requests;
using Common.Model.Responses;
using Common.Model;

namespace Dispatcher.Services
{
    public interface IPingService
    {
        Task<IActionResult> HandlePing(PingRequest request, string id);
    }

    public class PingService : IPingService
    {
        private readonly ILogger<PingService> _logger;

        public PingService(ILogger<PingService> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> HandlePing(PingRequest request, string id)
        {
            _logger.LogInformation("Processing ping request with ID: {Id}", id);
            
            // Simple ping implementation that returns a success response
            return new OkObjectResult(new JsonRpcSuccessResponse<PingResponse>
            {
                Id = id,
                Result = new PingResponse
                {
                    Message = "pong"
                }
            });
        }
    }
}