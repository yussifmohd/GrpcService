using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcService.Interceptors
{
    public class ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger) : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                logger.LogInformation("Server Intercepting Here!!!!");
                return await continuation(request, context);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
