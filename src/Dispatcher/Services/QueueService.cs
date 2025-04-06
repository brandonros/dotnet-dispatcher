using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dispatcher.Services
{
    public interface IQueueService<TRequest, TResponse> 
        where TRequest : class
        where TResponse : class
    {
        Task<Response<TResponse>> RequestResponse(TRequest request);
    }

    public class QueueService<TRequest, TResponse> : IQueueService<TRequest, TResponse> 
        where TRequest : class
        where TResponse : class
    {
        private readonly IRequestClient<TRequest> _client;
        private readonly ILogger<QueueService<TRequest, TResponse>> _logger;

        public QueueService(IRequestClient<TRequest> client, ILogger<QueueService<TRequest, TResponse>> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<TResponse>> RequestResponse(TRequest request)
        {
            _logger.LogInformation("Sending request of type {RequestType}", typeof(TRequest).Name);

            var requestHandle = _client.Create(request);
            requestHandle.TimeToLive = TimeSpan.FromSeconds(3600);
            var result = await requestHandle.GetResponse<TResponse>();
            
            _logger.LogInformation("Received response of type {ResponseType}", typeof(TResponse).Name);
            
            return result;
        }
    }
}
