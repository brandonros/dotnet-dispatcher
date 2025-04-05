using Common.Model.JsonRpc;
using Common.Model.Requests;
using Common.Model.Responses;
using MassTransit;

namespace Consumer.Handlers;

public class GetUserJsonRpcRequestHandler : IConsumer<JsonRpcRequest<GetUserRequest>>
{
    private readonly ILogger<GetUserJsonRpcRequestHandler> _logger;

    public GetUserJsonRpcRequestHandler(ILogger<GetUserJsonRpcRequestHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<JsonRpcRequest<GetUserRequest>> context)
    {
        _logger.LogInformation("Received GetUserRequest with ID: {RequestId}", context.Message.Id);
        
        // Extract the actual request from the wrapper
        var userRequest = context.Message.Params;
        
        // Process the request
        // For example: var user = await _userRepository.GetUserAsync(userRequest.UserId);
        
        // Create a response - Note we're using JsonRpcSuccessResponse explicitly
        var response = new JsonRpcSuccessResponse<GetUserResponse>
        {
            Id = context.Message.Id,  // Use the request ID, not the userId
            JsonRpc = "2.0",
            Result = new GetUserResponse
            {
                UserId = userRequest.UserId,
                Name = "Example User",
                Email = "user@example.com"
            }
        };
        
        // Send the response back using the concrete type
        await context.RespondAsync<JsonRpcSuccessResponse<GetUserResponse>>(response);
    }
}