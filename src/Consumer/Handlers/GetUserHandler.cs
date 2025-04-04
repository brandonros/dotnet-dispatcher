using Common.Model;
using Common.Model.Requests;
using Common.Model.Responses;
using MassTransit;

namespace Consumer.Handlers;
    
public class GetUserHandler : IConsumer<GetUserJsonRpcRequest>
{
    private readonly ILogger<GetUserHandler> _logger;

    public GetUserHandler(
        ILogger<GetUserHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetUserJsonRpcRequest> context)
    {
        _logger.LogInformation("Received JsonRpc request with ID: {Id}", context.Message.Id);

        try
        {
            // Now we can access the params directly since it's strongly typed
            var getUserParams = context.Message.Params;

            var result = new GetUserResponse
            {
                UserId = getUserParams.UserId,
                Name = "John Doe",
                Email = "john.doe@example.com"
            };

            var jsonRpcResponse = new JsonRpcSuccessResponse<GetUserResponse>
            {
                Id = context.Message.Id,
                Result = result
            };

            await context.RespondAsync(jsonRpcResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GetUser request");
            
            await context.RespondAsync(new JsonRpcErrorResponse
            {
                Id = context.Message.Id,
                Error = new JsonRpcError
                {
                    Code = -32603,  // Internal error
                    Message = "Failed to get user"
                }
            });
        }
    }
}