using MassTransit;
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
        private readonly IBus _bus;
        private readonly IRequestClient<TRequest> _client;

        public QueueService(IBus bus, IRequestClient<TRequest> client)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Response<TResponse>> RequestResponse(TRequest request)
        {
            var response = _client.Create(request);
            return await response.GetResponse<TResponse>();
        }
    }
}
