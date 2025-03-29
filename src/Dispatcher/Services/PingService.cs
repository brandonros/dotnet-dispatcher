using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Dispatcher.Model;

namespace Dispatcher.Services
{
    public interface IPingService
    {
        Task<IActionResult> HandlePing(JsonElement requestElement, string id);
    }

    public class PingService : IPingService
    {
        public async Task<IActionResult> HandlePing(JsonElement requestElement, string id)
        {
            // Simple ping implementation that returns a success response
            return new OkObjectResult(new JsonRpcSuccessResponse<string>
            {
                Id = id,
                Result = "pong"
            });
        }
    }
}