using Grpc.Core.Interceptors;

namespace ApiClient.Interceptors
{
    public class ClientLoggerInterceptor(ILoggerFactory loggerFactory) : Interceptor
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<ClientLoggerInterceptor>();

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                _logger.LogInformation($"Starting the client Yousef call of type: {context.Method.FullName}, {context.Method.Type}");
                return continuation(request, context);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
