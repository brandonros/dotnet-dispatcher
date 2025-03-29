using Microsoft.AspNetCore.Mvc;
using Dispatcher.Model;
using Dispatcher.Model.Requests;
using Dispatcher.Model.Responses;

namespace Dispatcher.Services
{
    public interface IPingService
    {
        Task<IActionResult> HandlePing(PingRequest request, string id);
    }

    public class PingService : IPingService
    {
        public async Task<IActionResult> HandlePing(PingRequest request, string id)
        {
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