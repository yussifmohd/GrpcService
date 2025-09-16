using Basics;
using Microsoft.AspNetCore.Mvc;

namespace ApiClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrpcClientController(FirstServiceDefinition.FirstServiceDefinitionClient client) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var response = client.Unary(new Request
            {
                Content = "Hello from gRPC Client"
            });
            return Ok(response);
        }
    }
}
