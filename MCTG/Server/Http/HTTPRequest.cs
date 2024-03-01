using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Server.Http
{
    public class HTTPRequest
    {
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public Dictionary<string, string> HeaderParameter { get; set; } = new Dictionary<string, string>();
        public string? Content { get; set; } //Content can be null
        private StreamReader _reader;

        public HTTPRequest(StreamReader reader) 
        { 
            _reader = reader;
        }

        public void ParseRequ()
        {
            Console.WriteLine("\n<----Request---->");
            //reading the request method and path
            string? line = _reader.ReadLine();
            if (line != null)
            {
                string[] parts = line.Split(' ');
                Method = parts[0];
                Path = parts[1];
                Console.WriteLine(line);
            }
            bool isBody = false;
            int contentLength = 0;

            while((line = _reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
                if(string.IsNullOrEmpty(line)) // = ""
                {
                    isBody = true;
                    break;
                }

                if(!isBody)
                {
                    var headerParts = line.Split(':', 2);
                    if(headerParts.Length == 2) 
                    {
                        if (headerParts[0] == "Content-Length")
                        {
                            //Console.WriteLine($"Content-Length: {headerParts[1]}");
                            contentLength = int.Parse(headerParts[1].Trim());
                        }
                        if (headerParts[0] == "Authorization")
                        {
                            headerParts[1] = headerParts[1].Trim();
                        }
                        /* if (headerParts[0] == "CURL-Fail")
                         {
                             headerParts[1] = headerParts[1].Trim();
                         }
                         if (headerParts[0] == "CURL-Test")
                         {
                             headerParts[1] = headerParts[1].Trim();
                         }*/
                        if (headerParts[0] == "Deck-Config")
                        {
                            headerParts[1] = headerParts[1].Trim();
                        }
                        HeaderParameter[headerParts[0].Trim()] = headerParts[1].Trim();
                    }
                }
            }
            //read body content if existing
            if (contentLength > 0)
            {
                var buffer = new StringBuilder(200);
                char[] charArr = new char[1024]; //new char[contentLength];?
                int readBytes = 0;
                while(readBytes < contentLength)
                {
                    var currReadBytes = _reader.Read(charArr, 0, charArr.Length);
                    readBytes += currReadBytes;
                    if(currReadBytes == 0) 
                    {
                        break;
                    }
                    buffer.Append(charArr, 0, currReadBytes);
                }
                Console.WriteLine(buffer.ToString());
                Content = buffer.ToString();
            }
        }
    }
}
