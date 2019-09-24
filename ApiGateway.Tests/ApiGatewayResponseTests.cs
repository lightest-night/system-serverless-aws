using System;
using System.Net;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace LightestNight.System.Serverless.AWS.ApiGateway.Tests
{
    public class ApiGatewayResponseTests
    {
        [Fact]
        public void Should_Create_Conflict_Response()
        {
            // Arrange
            const string message = "Test Conflict Response";
            
            // Act
            var result = ApiGatewayResponse.Conflict(message);
            
            // Assert
            result.StatusCode.ShouldBe((int) HttpStatusCode.Conflict);
            result.Body.ShouldBe(JsonConvert.SerializeObject(message));
        }

        [Fact]
        public void Should_Create_InternalServerError_Response()
        {
            // Arrange
            var exception = new Exception("Test Message");
            
            // Act
            var result = ApiGatewayResponse.InternalServerError(exception);
            
            // Assert
            result.StatusCode.ShouldBe((int) HttpStatusCode.InternalServerError);
            var message = (string) JsonConvert.DeserializeObject<dynamic>(result.Body).Message;
            message.ShouldBe(exception.Message);
        }

        [Fact]
        public void Should_Create_OK_Response()
        {
            // Arrange
            var testObject = new TestObject {Foo = "Test"};
            
            // Act
            var result = ApiGatewayResponse.Ok(testObject);
            
            // Assert
            result.StatusCode.ShouldBe((int) HttpStatusCode.OK);
            result.Body.ShouldBe(JsonConvert.SerializeObject(testObject));
        }

        [Fact]
        public void Should_Create_NoContent_Response()
        {
            // Act
            var result = ApiGatewayResponse.NoContent();
            
            // Assert
            result.StatusCode.ShouldBe((int) HttpStatusCode.NoContent);
            result.Body.ShouldBeNull();
        }
    }
}