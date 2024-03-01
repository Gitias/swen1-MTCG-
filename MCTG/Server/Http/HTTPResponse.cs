using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Server.Http
{
    public class HTTPResponse
    {
        private StreamWriter _writer;
        public int ReturnCode { get; set; } = 200;
        public string ReturnMessage { get; set; } = "ok";
        public Dictionary<string, string> HeaderParameter { get; private set; } = new Dictionary<string, string>();
        public string? Content { get; set; } //Content can be null

        public HTTPResponse(StreamWriter writer)
        {
            _writer = writer;
        }

        public void SendResponse()
        {
            Print();
            _writer.WriteLine($"HTTP/1.1 {ReturnCode} {ReturnMessage}\r\n");

            if(Content != null)
            {
                HeaderParameter["Content-Length"] = Content.Length.ToString();
            }
            foreach(var header in HeaderParameter)
            {
                _writer.WriteLine($"{header.Key}:{header.Value}");
            }
            _writer.WriteLine(); //sending empty line to signal end of header
            if(Content !=null)
            {
                _writer.WriteLine($"{Content}");
            }
            _writer.Flush();
            _writer.Close();
        }

        public void Reset()
        {
            //resets the state of the HTTPResponse object
            ReturnCode = 200;
            ReturnMessage = "OK";
            HeaderParameter.Clear();
            Content = null;
        }
        private void Print()
        {
            Console.WriteLine("\n<----Response---->");
            Console.WriteLine($"ReturnCode: {ReturnCode}");
            Console.WriteLine($"ReturnMessage: {ReturnMessage}");
            foreach (var header in HeaderParameter)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }
            if (Content != null)
            {
                Console.WriteLine($"{Content}");
            }
            Console.WriteLine("\n<-------------------------End-------------------->\n");
        }
    }
}
