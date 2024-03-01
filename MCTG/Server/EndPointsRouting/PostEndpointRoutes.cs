using MCTG.Data;
using MCTG.Generator;
using MCTG.Models;
using MCTG.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCTG.Server.EndPointsRouting
{
    internal class PostEndpointRoutes
    {
        public Dictionary<string, Action<HTTPRequest, HTTPResponse>> postMap = new();
        public PostEndpointRoutes()
        {
            //postMap
            postMap["/users"] = (request, response) => PostUser(request, response);
            postMap["/sessions"] = (request, response) => PostSession(request, response);
            postMap["/packages"] = (request, response) => PostPackages(request, response);
            postMap["/battles"] = (request, response) => PostBattles(request, response);
            postMap["/battles/{battleid}"] = (request, response) => PostBattles(request, response);
            //postMap["/tradings"] = (rq, rs) => PostTradingDeal(rq, rs);
            //postMap["/tradings/{tradingdealid}"] = (rq, rs) => PostTradings(rq, rs);
            postMap["/logout"] = (request, response) => PostLogout(request, response);
            //postMap["/skill-chips"] = (rq, rs) => PostSkillChips(rq, rs);
        }
        //usually private and static with map but publiv fot testing.
        private static void PostUser(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                //deerialize JSON-String, ?? --> if null uses "" 
                var user = JsonSerializer.Deserialize<Credentials>(request.Content ?? "");
                Console.WriteLine(request.Content);
                Console.WriteLine($"Deserialized User: Username={user?.Username}, Password={user?.Password}");
                if (user != null)
                {
                    UserRepository usr = new UserRepository();
                    usr.Add(user);
                    response.ReturnCode = 201;
                    response.ReturnMessage = "User Createt successfuly";
                }
                else
                {
                    throw new BadRequEx();
                }
            }
            catch
            {
                throw;
            }
        }
        private static void PostSession(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                var user = JsonSerializer.Deserialize<Credentials>(request.Content ?? "");
                if(user != null)
                {
                    UserRepository usr = new();
                    usr.LoginUser(user);
                    response.ReturnCode = 210;
                    response.ReturnMessage = "User Registered or Logged in successfuly";
                }
                else
                {
                    throw new BadRequEx();
                }
            }
            catch
            {
                throw;
            }
        }

        private static void PostLogout(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    UserRepository usr = new();
                   
                    usr.LogOutuser(uId);
                    response.ReturnCode = 204;
                    response.ReturnMessage = "User logged out successfuly";
                   
                }
                else
                {
                    throw new UnauthorizedAccessException("No authorization");
                }
            }
            catch
            {
                throw;
            }
        }
        private static void PostPackages(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    UserRepository usr = new();
                    if(usr.PayForPackage(uId))
                    {
                        CardRepository cardRepo = new();
                        Package package = PackageGenerator.CreatePackage();
                        cardRepo.Add(package, uId);
                        response.ReturnCode = 205;
                        response.ReturnMessage = "User Package succesfully";
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("No authorization");
                }
            }
            catch
            {
                throw;
            }
        }
        private static void PostBattles(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    string LobbyID = null;
                    var parts = request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if(parts.Length == 2 && parts[1] != null)
                    {
                        LobbyID = parts[1];
                    }
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    BattleRepository battleRepo = new();
                    battleRepo.Initiatelobby(uId, LobbyID);
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
