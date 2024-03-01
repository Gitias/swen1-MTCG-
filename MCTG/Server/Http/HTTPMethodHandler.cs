using MCTG.Interfaces;
using MCTG.Server.EndPointsRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Server.Http
{
    public class HTTPMethodHandler
    {
        internal static void HandleRequest(HTTPRequest request, HTTPResponse response)
        {
            string currentPath = request.Path;
            Dictionary<string, Action<HTTPRequest, HTTPResponse>> MethodRouting = GetMethods(request.Method);

            var parts = currentPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2 && parts[0] == "users")
            {
                currentPath = "/users/{username}";
            }
            else if (parts.Length == 2 && parts[0] == "battles")
            {
                currentPath = "/battles/{battleid}";
            }
            /*else if (parts.Length == 2 && parts[0] == "tradings")
             * {
             * 
                currentPath = "/tradings/{tradingdealid}";
                 }*/
            try
            {
                if (MethodRouting.TryGetValue(currentPath, out var handle)) //if key was found, dynamically call "Action" with  Values
                {
                    handle.DynamicInvoke(request, response);
                }
                else
                {
                    throw new NotFoundEx("Path not found");
                }

            }
            catch (Exception exception) 
            {
                if(exception is IException selfmadeEx)
                {
                    response.ReturnCode = selfmadeEx.StatusCode;
                    response.ReturnMessage = selfmadeEx.ErrorMessage;
                }
                else if (exception.InnerException is IException selfmadeInnerEx)
                {
                    response.ReturnCode = selfmadeInnerEx.StatusCode;
                    response.ReturnMessage = selfmadeInnerEx.ErrorMessage;
                }
                else
                {
                    Console.WriteLine(exception.Message);
                    throw;
                }
            }
        }
      
        internal static Dictionary<string, Action<HTTPRequest, HTTPResponse>> GetMethods(string method)
        {
            switch (method.ToUpper())
            {
                case "GET":
                    GetEndpointRoutes get = new();
                    return get.getMap;
                case "PUT":
                    PutEndpointRoutes put = new();
                    return put.putMap;
                case "DELETE":
                    DelEndpointRoutes del = new();
                    return del.delMap;
                case "POST":
                    PostEndpointRoutes post = new();
                    return post.postMap;
                default:
                    throw new BadRequEx($"Unsupported HTTP method: {method}");
            }
        }

    }
}
