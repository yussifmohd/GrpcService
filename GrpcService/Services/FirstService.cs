using Basics;
using Grpc.Core;
using GrpcService.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GrpcService.Services
{
    public class FirstService : FirstServiceDefinition.FirstServiceDefinitionBase, IFirstService
    {
        [Authorize(Roles = "Admin")]
        public override Task<Response> Unary(Request request, ServerCallContext context)
        {
            ////Used To Test The Retry Policy & Hedging Policy
            //if (!context.RequestHeaders.Where(k => k.Key == "grpc-previous-rpc-attempts").Any())
            //{
            //    throw new RpcException(new Status(StatusCode.Internal, "Not Here, Try Again!!"));
            //}


            //To Override Compression for this call only
            //context.WriteOptions = new WriteOptions(WriteFlags.NoCompress);

            var response = new Response
            {
                Message = request.Content + $" from server !!!!!!!!!!!!!!!!"
            };
            Console.WriteLine(response.Message);
            return Task.FromResult(response);
        }

        public override async Task<Response> ClientSteam(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
        {
            var response = new Response()
            {
                Message = "I got "
            };

            while (await requestStream.MoveNext())
            {
                Request requestPayload = requestStream.Current;
                Console.WriteLine(requestPayload);
                response.Message += requestPayload.Content + " ";
            }

            return response;
        }

        public override async Task ServerStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {

            var headers = context.RequestHeaders.Get("my-first-key");
            string? headersValue = headers?.Value;
            for (int i = 0; i < 100; i++)
            {
                if (context.CancellationToken.IsCancellationRequested) return;

                var response = new Response
                {
                    Message = request.Content + " from server " + i
                };

                await responseStream.WriteAsync(response);
            }

            //Similar To Response Headers --> Sent only after streaming is done
            var trailers = new Metadata.Entry("my-trailer", "my-trailer-value");
            context.ResponseTrailers.Add(trailers);
        }

        public override async Task BiDirectionalStream(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {
            var response = new Response()
            {
                Message = "Test Response"
            };
            while (await requestStream.MoveNext())
            {
                var requestPayload = requestStream.Current;
                response.Message = requestPayload.ToString();
                await responseStream.WriteAsync(response);
            }
        }
    }
}
