using MCTG;
using MCTG.Models;
using MCTG.Server;
using MCTG.Server.EndPointsRouting;
using MCTG.Server.Http;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Test
{
    public class ServerLogicTests
    {
        [Test] //#8
        public void TestAuthenticationCheck_withInvalidToken_ShouldThrowEcxeption()
        {
            //Arrange Act Assert
            Assert.Throws<UnauthorizedEx>(() => Authenticator.AuthenticationCheck("Inavlid Token"), "Unable to get authorization status");
        }

        [Test] //#9
        public void TestResponse_WithInvalidToken_ReturnsUnauthorizedEx()
        {
            // Arrange
            string body_Request = "This is the body of the reqest";
            int contentLength = Encoding.UTF8.GetBytes(body_Request).Length;
            string validRequest = $"GET /users/username HTTP/1.1\r\nContent-Length: {contentLength}\r\nAuthorization: Non-existing Bearer\r\n\r\n" + body_Request;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(validRequest));
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            var httpRequest = new HTTPRequest(reader);
            var httpResponse = new HTTPResponse(writer);
            //Act
            httpRequest.ParseRequ();

            HTTPMethodHandler.HandleRequest(httpRequest, httpResponse);

            //Assert
            Assert.That(httpResponse.ReturnCode, Is.EqualTo(401));
            Assert.That(httpResponse.ReturnMessage, Is.Not.EqualTo("OK"));
        }
        [Test] //#10
        public void TestResponse_WithInvalidPath_ReturnsNotFoundEx()
        {
            // Arrange
            string body_Request = "This is the body of the reqest";
            int contentLength = Encoding.UTF8.GetBytes(body_Request).Length;
            string validRequest = $"GET /WrongPath HTTP/1.1\r\nContent-Length: {contentLength}\r\nAuthorization: TestUser Bearer\r\n\r\n"+ " " + body_Request;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(validRequest));
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            var httpRequest = new HTTPRequest(reader);
            var httpResponse = new HTTPResponse(writer);
            //Act
            httpRequest.ParseRequ();

            HTTPMethodHandler.HandleRequest(httpRequest, httpResponse);

            //Assert
            Assert.That(httpResponse.ReturnCode, Is.EqualTo(404), "Not Responded with 404 NotFoundEx"); //NotFoundEx
            Assert.That(httpResponse.ReturnMessage, Is.Not.EqualTo("OK"));
        }

        [Test] //#11
        public void TestDeckConfig_lessThan4Cards()
        {
            //Arrange
            string body_Request = "[\"41364231-3d58-4fc9-bd17-89070a07a025\", \"286f889a-63b9-45d6-8c0d-affd4131075a\", \"b6413e0d-6bc4-453b-a43b-b8bb37431a39\"]";

            int contentLength = Encoding.UTF8.GetBytes(body_Request).Length;
            string validRequest = $"PUT /deck HTTP/1.1\r\n" +
                                  "Header1: Value1\r\n" +
                                  "Header2: Value2\r\n" +
                                  $"Content-Length: {contentLength}\r\n" +
                                  "Authorization: Bearer TestUser-mtcgToken\r\n" +
                                  "\r\n" +
                                  "" +
                                  body_Request;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(validRequest));
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            var httpRequest = new HTTPRequest(reader);
            var httpResponse = new HTTPResponse(writer);

            //Act
            httpRequest.ParseRequ();
            HTTPMethodHandler.HandleRequest(httpRequest, httpResponse);

            //Assert
            Assert.That(httpResponse.ReturnCode, Is.EqualTo(400), "Couldn't get BadRequEx");
            Assert.That(httpResponse.ReturnMessage, Is.EqualTo("Selection contains too few cards"), "Couldn't get BadRequEx");
            Assert.That(httpResponse.ReturnCode, Is.Not.EqualTo(null), "Couldn't get BadRequEx");
        }

        [Test] //#12
        public void TestDeckConfig_moreThan4Cards()
        {
            //Arrange
            string body_Request = "[\"41364231-3d58-4fc9-bd17-89070a07a025\", \"286f889a-63b9-45d6-8c0d-affd4131075a\", \"b6413e0d-6bc4-453b-a43b-b8bb37431a39\", \"7a56d533-6640-4c8c-a07e-2e0653ea952f\", \"e4e67435-1cae-4727-9006-5c07f3912f9f\"]";

            int contentLength = Encoding.UTF8.GetBytes(body_Request).Length;
            string validRequest = $"PUT /deck HTTP/1.1\r\n" +
                                  "Header1: Value1\r\n" +
                                  "Header2: Value2\r\n" +
                                  $"Content-Length: {contentLength}\r\n" +
                                  "Authorization: Bearer TestUser-mtcgToken\r\n" +
                                  "\r\n" +
                                  "" +
                                  body_Request;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(validRequest));
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            var httpRequest = new HTTPRequest(reader);
            var httpResponse = new HTTPResponse(writer);

            //Act
            httpRequest.ParseRequ();
            HTTPMethodHandler.HandleRequest(httpRequest, httpResponse);

            //Assert
            Assert.That(httpResponse.ReturnCode, Is.EqualTo(400), "Not a BadRequestEx");
            Assert.That(httpResponse.ReturnMessage, Is.EqualTo("Selection contains too many cards"));
            Assert.That(httpResponse.ReturnCode, Is.Not.EqualTo(null), "Couldn't get BadRequEx");
        }

        [TestCase("GET", typeof(GetEndpointRoutes))] //#13 (could argue 13-15)
        [TestCase("PUT", typeof(PutEndpointRoutes))]
        [TestCase("POST", typeof(PostEndpointRoutes))]
        public void TestGetMethodMap_withValidMethod_ReturnsCorrectMap(string method, Type EndpointType_expected)
        {
            //Arrange Act
            var methodMap = HTTPMethodHandler.GetMethods(method); //Gets method map of "type"

            //Asserts
            Type actualType = GetEndpointInstance(methodMap)?.GetType().DeclaringType; //Gets Type of methodMap
            Type TestType = EndpointType_expected;

            Assert.IsTrue(
                actualType != null &&
                (actualType.FullName == TestType.FullName || actualType.BaseType == TestType),
                $"Expected {EndpointType_expected.Name}, but got {actualType?.Name}");
        }

        [Test] //# 14
        public void TestGetMethodMap_WithInvalidMethod_ThrowBadRequestException()
        {
            Assert.Throws<BadRequEx>(() => HTTPMethodHandler.GetMethods("INVALID_METHOD"), "Couldt fetch BadRequEx");
        }

        [Test] //# 15 //Only works if TestUser is logg in
        public void AuthenticationCheck_WithKnownToken_ReturnsExpectedResult()
        {
            // Arrange
            string testToken = "Bearer TestUser-mtcgToken";
            int expectedUserId = 3; 
            bool expectedIsActive = true;

            // Act
            var result = Authenticator.AuthenticationCheck(testToken);

            // Assert
            Assert.AreEqual(expectedIsActive, result.isTokenActive);
            Assert.AreEqual(expectedUserId, result.userId);
        }

        [Test] //# 16 
        public void AuthenticationCheck_WithWrongToken_UnauthorizedEx()
        {
            // Arrange
            string testToken = "Bearer WRONGTOKEN-mtcgToken";

            // Assert Act
            Assert.Throws<UnauthorizedEx>(()=>Authenticator.AuthenticationCheck(testToken), "Unable to throw UnauthorizedEx");
        }






        //returns Instance of method
        private object GetEndpointInstance(Dictionary<string, Action<HTTPRequest, HTTPResponse>> map)
        {
            // Helper method to get the instance of the endpoint class
            if (map is not null && map.Count > 0)
            {
                foreach (var key_value_pair in map)
                {
                    return key_value_pair.Value.Target; // Assumes that the endpoint instance is the target
                }
            }
            return null;
        }
    }
}