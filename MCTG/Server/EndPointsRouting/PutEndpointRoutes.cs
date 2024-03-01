using MCTG.Data;
using MCTG.Logic;
using MCTG.Models;
using MCTG.Server.Http;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCTG.Server.EndPointsRouting
{
    internal class PutEndpointRoutes
    {
        public Dictionary<string, Action<HTTPRequest, HTTPResponse>> putMap = new();
        public PutEndpointRoutes()
        {
            //putMap
            putMap["/users/{username}"] = (request, response) => PutUser(request, response);
            putMap["/deck"] = (request, response) => PutDeck(request, response);
            putMap["/battles"] = (request, response) => PutJoinLobby_StartBattle(request, response);
            
        }
        private static void PutUser(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    var parts = request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if(parts.Length == 2)
                    {
                        var UserData = JsonSerializer.Deserialize<UserData>(request.Content ?? "");
                        Console.WriteLine(UserData);
                        if (parts[1] != null)
                        {
                            (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                            UserRepository usr = new();
                            User? user = usr.GetUserByUsername(parts[1]);
                            if (user == null)
                            {
                                throw new NotFoundEx("User not found");
                            }
                            else if(user.UserId != uId)
                            {
                                throw new AccsessEx("Unable to edit someone's elses profile");
                            }
                            usr.Insert_Edit_UserProfile(user.UserId, UserData);
                            response.ReturnCode = 209;
                            response.ReturnMessage = "Succesfully changed you user data";
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
        private static void PutDeck(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    List<string> deck = new();
                    Random rand = new();
                    
                    if (request.HeaderParameter.ContainsKey("Deck-Config")) 
                    {
                        //OPtion to autoconfigure your deck
                        if (request.HeaderParameter["Deck-Config"] == "true")
                        {
                            CardRepository cardRepo = new CardRepository();
                            Stack userCards = cardRepo.GetUserStack(uId);
                            HashSet<int> selectedIndices = new HashSet<int>(); // Uses hash Set to avoid duplicats
                            Console.WriteLine("\n Automatically Configured Deck \n");
                            //Resets current Deck
                            cardRepo.ResetDeck(uId);
                            while (selectedIndices.Count < 4) 
                            {
                                int RandomIndex = rand.Next(userCards.cards.Count);
                                if (selectedIndices.Add(RandomIndex)) //Adds index to HasSet (only works if new index)
                                {
                                    deck.Add(userCards.cards[RandomIndex].Id);
                                    Console.WriteLine(userCards.cards[RandomIndex].Name + ", Damage: " + userCards.cards[RandomIndex].Damage);
                                }    
                            }
                        }
                        else
                        {
                            deck = JsonSerializer.Deserialize<List<string>>(request.Content ?? "");
                        }
                    }
                    else
                    {
                        deck = JsonSerializer.Deserialize<List<string>>(request.Content ?? "");
                    }
                    if (deck.Count == 4)
                    {
                        CardRepository cardRepo = new CardRepository();
                        cardRepo.EditDeck(deck, uId);
                        response.ReturnCode = 216;
                        response.ReturnMessage = "Succesfully changed your Deck";
                    }
                    else if (deck.Count > 4)
                    {
                        throw new BadRequEx("Selection contains too many cards");
                    }
                    else if (deck.Count < 4)
                    {
                        throw new BadRequEx("Selection contains too few cards");
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
        private static void PutJoinLobby_StartBattle(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (request.HeaderParameter.ContainsKey("Authorization"))
                {
                    string Lobby_ID = "";
                    (bool isTokenActive, int uId) = Authenticator.AuthenticationCheck(request.HeaderParameter["Authorization"]); //Throws exception if not active
                    //Could rewrite in order to automatically join lobby without needing iD
                    var parts =  request.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 1) 
                    {
                        Random rand = new();
                        BattleRepository Battle_Repo = new BattleRepository();
                        int RandomIndex;
                        int retryCount = 0;
                        List<Battle> lobbieIds = Battle_Repo.GetAvailableLobbies();
                        do
                        {
                            RandomIndex = rand.Next(lobbieIds.Count);
                            retryCount++;
                        } while (lobbieIds[RandomIndex].User_id == uId && retryCount < 40);

                        if(retryCount == 40)
                        {
                            throw new AccsessEx("Couldn't find open lobby");
                        }

                        Lobby_ID = lobbieIds[RandomIndex].Id;
                    }
                    else if (parts.Length == 2 && parts[1] != null)
                    {
                        Lobby_ID = parts[1];
                    }
                    else
                    {
                        throw new BadRequEx("Request Parameter Error");
                    }
                    
                        BattleRepository battleRepo = new BattleRepository();
                        Battle Lobby = battleRepo.GetLobbyById(Lobby_ID);
                        if(uId != Lobby.User_id)
                        {
                            CardRepository cardRepo = new CardRepository();
                            UserRepository userRepo = new UserRepository();
                            User Player = userRepo.GetUserById(Lobby.User_id);
                            User Opponent = userRepo.GetUserById(uId);
                            if(Player != null && Opponent != null)
                            {
                                Deck PlayerDeck = new()
                                {
                                    deck = cardRepo.GetUserDeck(Player.UserId),
                                    UserId = Player.UserId,
                                    Name = Player.Username
                                };

                                Deck OpponentDeck = new()
                                {
                                    deck = cardRepo.GetUserDeck(Opponent.UserId),
                                    UserId = Opponent.UserId,
                                    Name = Opponent.Username
                                };

                                if (battleRepo.JoinOpenLobby(Lobby_ID, uId))
                                {
                                    GameLogic game = new GameLogic();
                                    (bool draw, int winnerId, int loserId) = game.Battle(PlayerDeck, OpponentDeck);
                                    userRepo.UpdateElo(winnerId, loserId, draw);
                                    battleRepo.EvaluateBattle(Lobby_ID, draw, winnerId);
                                    response.ReturnCode = 217;
                                    response.ReturnMessage = "Succesfully carried out battle";
                            }
                                else
                                {
                                    throw new AccsessEx("Cannot join lobby");
                                }
                            }
                            else
                            {
                                throw new Exception("Player or opponent is null");
                            }
                        }
                        else
                        {
                            throw new AccsessEx("Cannot start battle with yourself");
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
    }
}
