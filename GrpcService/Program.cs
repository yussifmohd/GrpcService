using Auth;
using GrpcService.Interceptors;
using GrpcService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ServerLoggerInterceptor>();
    options.ResponseCompressionAlgorithm = "gzip";
    options.ResponseCompressionLevel = CompressionLevel.SmallestSize;

    ////used if we create a custom compression provider
    //options.CompressionProviders =
    //[
    //    new GzipCompressionProvider(CompressionLevel.SmallestSize)
    //];
}).AddJsonTranscoding();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateActor = false,
        ValidateLifetime = true,
        IssuerSigningKey = JwtHelper.SecurityKey
    });

builder.Services.AddAuthorization(options => options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
{
    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
    policy.RequireClaim(ClaimTypes.Name);
}));

builder.Services.AddGrpcHealthChecks(options =>
{

}).AddCheck("My Cool Service", () => HealthCheckResult.Healthy(), ["grpc", "live"]);

builder.Services.AddGrpcReflection();

var app = builder.Build();

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<FirstService>();

app.MapGrpcHealthChecksService();
app.MapGrpcReflectionService();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

//For Unit Tests
public partial class Program { }