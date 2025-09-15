


using Basics;
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;

//Send Retry Requests whenver find StatusCode.Internal
var retryPolicy = new MethodConfig
{
    Names = { MethodName.Default },
    RetryPolicy = new RetryPolicy
    {
        MaxAttempts = 5,
        BackoffMultiplier = 1,
        InitialBackoff = TimeSpan.FromSeconds(1),
        MaxBackoff = TimeSpan.FromSeconds(5),
        RetryableStatusCodes = { StatusCode.Internal }
    }
};

//Send Multiple Requests Until One Succeed, Then Other Requests Will Be Cancelled
var hedgingPolicy = new MethodConfig
{
    Names = { MethodName.Default },
    HedgingPolicy = new HedgingPolicy
    {
        MaxAttempts = 5,
        NonFatalStatusCodes = { StatusCode.Internal },
        HedgingDelay = TimeSpan.FromSeconds(0.5)
    }
};

var options = new GrpcChannelOptions
{
    ServiceConfig = new ServiceConfig
    {
        //MethodConfigs = { retryPolicy }
        MethodConfigs = { hedgingPolicy }
    }
};

using var channel = GrpcChannel.ForAddress("http://localhost:5084", options);

//HEALTH CHECKS
var health = new Health.HealthClient(channel);
var healthResult = health.Check(new HealthCheckRequest());

Console.WriteLine($"Health Status: {healthResult.Status}");


//var factory = new StaticResolverFactory(addr =>
//[
//    new BalancerAddress("localhost", 7095),
//    new BalancerAddress("localhost", 5084),
//]);

//var service = new ServiceCollection();
//service.AddSingleton<ResolverFactory>(factory);

//var channel = GrpcChannel.ForAddress("static://localhost", new GrpcChannelOptions
//{
//    Credentials = ChannelCredentials.Insecure,
//    HttpHandler = new SocketsHttpHandler
//    {
//        EnableMultipleHttp2Connections = true,
//    },
//    ServiceConfig = new ServiceConfig
//    {
//        LoadBalancingConfigs = { new RoundRobinConfig() }
//    },
//    ServiceProvider = service.BuildServiceProvider()
//});

var client = new FirstServiceDefinition.FirstServiceDefinitionClient(channel);


Unary(client);
//ClientStreaming(client);
//ServerStreaming(client);
//BiDirectionalStreaming(client);

Console.ReadLine();

void Unary(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    var metaData = new Metadata
    {
        { "grpc-accept-encoding", "gzip" }
    };

    var request = new Request { Content = "Hello from gRPC client!" };
    var response = client.Unary(request, /*deadline: DateTime.UtcNow.AddSeconds(3),*/ headers: metaData);
}

async void ClientStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    using var call = client.ClientSteam();
    for (int i = 0; i < 1000; i++)
    {
        await call.RequestStream.WriteAsync(new Request { Content = i.ToString() });
    }
    await call.RequestStream.CompleteAsync();
    var response = await call;
    Console.WriteLine($"{response.Message}");
}

async void ServerStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    try
    {
        var canellationToken = new CancellationTokenSource();

        //Similar Request Headers
        var metaData = new Metadata
        {
            { "my-first-key", "my-first-value" },
            { "my-second-key", "my-second-value" }
        };

        using var streamingCall = client.ServerStream(new Request { Content = "Hello!" }, headers: metaData);

        await foreach (var response in streamingCall.ResponseStream.ReadAllAsync(canellationToken.Token))
        {
            Console.WriteLine(response.Message);
            if (response.Message.Contains('1'))
            {
                canellationToken.Cancel();
            }
        }

        var trailers = streamingCall.GetTrailers();
        var trailerValue = trailers.GetValue("my-trailer");
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {

    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.PermissionDenied)
    {

    }
}

async void BiDirectionalStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    using var call = client.BiDirectionalStream();

    var request = new Request();
    for (int i = 0; i < 10; i++)
    {
        request.Content = i.ToString();
        Console.WriteLine(request.Content);
        await call.RequestStream.WriteAsync(request);
    }

    while (await call.ResponseStream.MoveNext())
    {
        var message = call.ResponseStream.Current;
        Console.WriteLine(message);
    }

    await call.RequestStream.CompleteAsync();
}