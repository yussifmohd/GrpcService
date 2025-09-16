using Basics;
using FluentAssertions;

namespace GrpcService.Tests.Integration
{
    public class FirstServiceTest(MyFactory<Program> factory) : IClassFixture<MyFactory<Program>>
    {
        private readonly MyFactory<Program> _factory = factory
;
        [Fact]
        public void GetUnaryMessage()
        {
            //Arrange
            var request = new Request
            {
                Content = "message"
            };

            var client = _factory.CreateGrpcClient();
            var expectedResponse = new Response
            {
                Message = request.Content + $" from server !!!!!!!!!!!!!!!!"
            };
            //Act
            var actualResponse = client.Unary(request);
            //Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}