using Basics;
using Grpc.Core;

namespace GrpcService.Interfaces
{
    public interface IFirstService
    {
        Task BiDirectionalStream(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context);
        Task<Response> ClientSteam(IAsyncStreamReader<Request> requestStream, ServerCallContext context);
        Task ServerStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context);
        Task<Response> Unary(Request request, ServerCallContext context);
    }
}