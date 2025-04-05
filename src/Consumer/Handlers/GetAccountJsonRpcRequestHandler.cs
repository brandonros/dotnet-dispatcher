using Common.Model;
using Common.Model.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Consumer.Handlers;

public class GetAccountJsonRpcRequestHandler : IConsumer<JsonRpcRequest<GetAccountRequest>>
{
    private readonly ILogger<GetAccountJsonRpcRequestHandler> _logger;

    public GetAccountJsonRpcRequestHandler(ILogger<GetAccountJsonRpcRequestHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<JsonRpcRequest<GetAccountRequest>> context)
    {
        _logger.LogInformation("Received GetAccountRequest with ID: {RequestId}", context.Message.Id);
        
        // Extract the actual request from the wrapper
        var accountRequest = context.Message.Params;
        
        // Process the request
        // For example: var account = await _accountRepository.GetAccountAsync(accountRequest.AccountId);
        
        // Create a response
        var response = new JsonRpcSuccessResponse<GetAccountResponse>
        {
            Id = context.Message.Id,
            JsonRpc = "2.0",
            Result = new GetAccountResponse
            {
                AccountId = accountRequest.AccountId,
                AccountName = "Example Account",
                AccountType = "Savings",
                Balance = 1000.00M,
                CurrencyCode = "USD",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };
        
        // Send the response back
        await context.RespondAsync(response);
    }
}
