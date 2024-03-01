using MCTG;
using MCTG.Server.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Test
{
    internal class HTTPTests
    {
        [Test] //#3
        public void Parse_Valid_Request_ReturnsCorrectlyParsedRequest()
        {
            //Arrange
            string body_Request = "This is the Body of the Test Request";
            //int conten_length = Encoding.UTF8.GetBytes(body_Request).Length;

            // Arrange
            var validRequestData = $"GET /home HTTP/1.1\r\nContent-Length: 27\r\nAuthorization: Bearer token123\r\n\r\n" + body_Request;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(validRequestData);
            writer.Flush();
            stream.Position = 0; // Reset the stream position to the beginning
            var reader = new StreamReader(stream);

            var httpRequest = new HTTPRequest(reader); 

            // Act
            httpRequest.ParseRequ();

            // Assert
            Assert.AreEqual("GET", httpRequest.Method);
            Assert.AreEqual("/home", httpRequest.Path);
            Assert.AreEqual("27", httpRequest.HeaderParameter["Content-Length"]);
            Assert.AreEqual("Bearer token123", httpRequest.HeaderParameter["Authorization"]);
            Assert.AreEqual("This is the Body of the Test Request", httpRequest.Content);
        }

        [Test] //#4
        public void HandleRequest_ThrowsExceptionOnBadFormat()
        {
            // Arrange
            var badRequestData = "BAD REQUEST DATA WITHOUT CORRECT FORMAT\r\n"; // BAd Request
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(badRequestData));
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            var httpRequest = new HTTPRequest(reader);
            var httpResponse = new HTTPResponse(writer);

            // Act & Assert
            Assert.Throws<BadRequEx>(() => HTTPMethodHandler.HandleRequest(httpRequest, httpResponse),
                "Method did not throw expected exception on bad request format.");
            
        }
    }
}
