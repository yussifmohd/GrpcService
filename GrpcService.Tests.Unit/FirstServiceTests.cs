using Basics;
using FluentAssertions;
using GrpcService.Interfaces;
using GrpcService.Services;
using GrpcService.Tests.Unit.Helpers;

namespace GrpcService.Tests.Unit
{
    public class FirstServiceTests
    {

        private readonly IFirstService sut;

        public FirstServiceTests()
        {
            sut = new FirstService();
        }

        [Fact]
        public async void Unary_ShouldReturn_An_Object()
        {
            //Arrange
            var request = new Request
            {
                Content = "message"
            };
            var callContext = TestServerCallContext.Create();

            var expectedResponse = new Response
            {
                Message = request.Content + $" from server !!!!!!!!!!!!!!!!"
            };
            //Act
            var actualResponse = await sut.Unary(request, callContext);
            //Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}