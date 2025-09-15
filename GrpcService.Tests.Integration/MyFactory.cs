using Basics;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcService.Tests.Integration
{
    public class MyFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.UseTestServer();

            //For Connection Strings and Database Configurations
            builder.ConfigureTestServices(services =>
            {
                // Configure test services here if needed
            });
        }

        public FirstServiceDefinition.FirstServiceDefinitionClient CreateGrpcClient()
        {
            var httpClient = CreateClient();
            var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions
            {
                HttpClient = httpClient
            });

            return new FirstServiceDefinition.FirstServiceDefinitionClient(channel);
        }
    }
}
