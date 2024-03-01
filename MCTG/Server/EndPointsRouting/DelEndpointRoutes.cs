using MCTG.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Server.EndPointsRouting
{
    internal class DelEndpointRoutes
    {
        public Dictionary<string, Action<HTTPRequest, HTTPResponse>> delMap = new();
        public DelEndpointRoutes()
        {
            //delMap
        
        }

    }
}
