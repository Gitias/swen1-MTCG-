using MCTG.Data;
using MCTG.Models;
using MCTG.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCTG.Server.EndPointsRouting
{
    internal class GetEndpointRoutes
    {
        public Dictionary<string, Action<HTTPRequest, HTTPResponse>> getMap = new();
        public GetEndpointRoutes()
        {
            //getMap
            getMap["/users/{username}"] = (request, response) => GetUser(request, response);
            getMap["/deck"] = (request, response) => GetDeck(request, response);
            getMap["/cards"] = (request, response) => GetCards(request, response);
            getMap["/battles"] = (request, response) => GetBattles(request, response);
            getMap["/stats"] = (request, response) => GetStats(request, response);
            getMap["/scoreboard"] = (request, response) => GetScoreboard(request, response);
        }
        private static void GetStats(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    if (isTokenActive)
                    {
                        UserRepository usr = new();
                        UserStats stats = usr.GetUserStatsById(uId);
                        stats.UserId = uId;
                        response.HeaderParameter["Content-Type"] = "application/json";
                        response.Content = JsonSerializer.Serialize(stats);
                        response.ReturnCode = 206;
                        response.ReturnMessage = "Stats loading successfuly";
                    }
                    else
                    {
                        throw new UnauthorizedEx("No Authorization token provided");
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        private static void GetScoreboard(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    UserRepository usr = new();
                    List<UserStats> scoreboard = usr.GetScoreboard();
                    response.HeaderParameter["Content-Type"] = "application/json";
                    response.Content = JsonSerializer.Serialize(scoreboard);
                    response.ReturnCode = 207;
                    response.ReturnMessage = "Scoreboard loading successfuly";
                }
                else
                {
                    throw new UnauthorizedEx("No Authorization token provided");
                }
            }
            catch
            {
                throw;
            }
        }
        private static void GetUser(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    User? user;

                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    var parts = request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        if (parts[1] != null)                        
                        {
                            UserRepository usr = new();
                            user = usr.GetUserByUsername(parts[1]);
                            if(user == null)
                            {
                                throw new NotFoundEx("User not found");
                            }
                            else if(user.UserId != uId)
                            {
                                throw new AccsessEx("Not allowed to acces someone else's user data");
                            }
                            else if(user != null && user.UserId == uId)
                            {
                                response.HeaderParameter["Content-Type"] = "application/json";
                                response.Content = JsonSerializer.Serialize(user);
                                response.ReturnCode = 208;
                                response.ReturnMessage = "successfuly got User";
                            }
                        }
                        else
                        {
                            throw new BadRequEx("Missing (/username)");
                        }

                    }
                    else
                    {
                        throw new BadRequEx("Request Parameter Error");
                    }

                }
                else
                {
                    throw new UnauthorizedEx("No Authorization token provided");
                }
            }
            catch
            {
                throw;
            }
        }
        private static void GetDeck(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    List<Card> UserDeck = new();
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    CardRepository cardRepo = new();
                    UserDeck = cardRepo.GetUserDeck(uId);
                    if (UserDeck.Count > 0)
                    {
                        response.HeaderParameter["Content-Type"] = "application/json";
                        response.Content = JsonSerializer.Serialize(UserDeck);
                        response.ReturnCode = 210;
                        response.ReturnMessage = "successfuly got Deck";
                    }
                    else
                    {
                        response.ReturnCode = 204;
                        response.ReturnMessage = "Deck is empty";
                    }
                }
                else
                    throw new UnauthorizedEx("No Authorizaion provided");
            }
            catch
            {
                throw;
            }
        }
        private static void GetCards(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    Stack UserCards = new();
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    CardRepository cardRepo = new();
                    UserCards = cardRepo.GetUserStack(uId);
                    if (UserCards.cards.Count > 0)
                    {
                        response.HeaderParameter["Content-Type"] = "application/json";
                        response.Content = JsonSerializer.Serialize(UserCards.cards);
                        response.ReturnCode = 211;
                        response.ReturnMessage = "successfuly got Cards";
                    }
                    else
                    {
                        response.ReturnCode = 204;
                        response.ReturnMessage = "Deck is empty";
                    }
                }
                else
                    throw new UnauthorizedEx("No Authorizaion provided");
            }
            catch
            {
                throw;
            }
        }
        private static void GetBattles(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    BattleRepository battleRepo = new();
                    List<Battle> Lobbies = battleRepo.GetAvailableLobbies();
                    response.HeaderParameter["Content-Type"] = "application/json";
                    response.Content= JsonSerializer.Serialize(Lobbies);
                    response.ReturnCode=212;
                    response.ReturnMessage = "Succefully got lobbies";
                }
                else
                    throw new UnauthorizedEx("No Authorizaion provided");
            }
            catch
            {
                throw;
            }
        }
    }
}
