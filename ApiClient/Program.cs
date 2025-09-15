using ApiClient.Interceptors;
using Auth;
using Basics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ClientLoggerInterceptor>();
builder.Services.AddGrpcClient<FirstServiceDefinition.FirstServiceDefinitionClient>(options =>
{
    options.Address = new Uri("https://localhost:7095");
})
    .AddCallCredentials((context, metadata) =>
    {
        var token = JwtHelper.GenerateJwtToken("Api");
        if(!string.IsNullOrEmpty(token))
        {
            metadata.Add("Authorization", $"Bearer {token}");
        }

        return Task.CompletedTask;
    })
    .AddInterceptor<ClientLoggerInterceptor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
