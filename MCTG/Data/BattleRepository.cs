using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MCTG.Models;

namespace MCTG.Data
{
    internal class BattleRepository
    {
        internal void Initiatelobby(int uId, string? LobbyId)
        {
            using var db = new DatabaseConnector();
            try
            {
                //lock
                db.StartTransaction();
                using var command = db.CreateCommand("INSERT INTO battles (id, user_id, is_active, is_completed) VALUES (@id, @user_id, @bool_active, @bool_completed)");
                string unique_lobbyId;
                if(LobbyId != null && LobbyId.Length <= 49)
                {
                    unique_lobbyId = LobbyId;
                }
                else if(LobbyId == null)
                {
                    unique_lobbyId = Guid.NewGuid().ToString(); //gloab unique Id
                }
                else
                {
                    throw new BadRequEx("Invalid LobbyId");
                }
                db.AddParameter(command, "id", DbType.String, unique_lobbyId);
                db.AddParameter(command, "user_id", DbType.Int32, uId);
                db.AddParameter(command, "bool_active", DbType.Boolean, false);
                db.AddParameter(command, "bool_completed", DbType.Boolean, false);
                command.ExecuteNonQuery();
                db.CommitTransaction();
            }
            catch(NpgsqlException exception)
            {
                db.RollbackTransaction();
                if (exception.SqlState == "23505") //unique violation errorcode
                {
                    throw new DatabaseEx("Lobby already exists");
                }
                else
                {
                    throw new DatabaseEx(exception.Message);
                }
            }
            catch
            {
                db.RollbackTransaction();
                throw;
            }
        }
        internal Battle GetLobbyById(string LobbyId)
        {
            using var db = new DatabaseConnector();
            try
            {
                if(LobbyId == null)
                {
                    throw new NotFoundEx("Could not find Lobby (id is null)");
                }
                else
                {
                    using var command = db.CreateCommand("SELECT * FROM battles WHERE id = @lobby_id");
                    db.AddParameter(command, "lobby_id", DbType.String, LobbyId);
                    //lock
                    using IDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Battle()
                        {
                            Id = reader.GetString(0),
                            User_id = reader.GetInt32(1),
                            Challenger_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                            is_Active = reader.GetBoolean(3),
                            is_Draw = reader.IsDBNull(4) ? null : reader.GetBoolean(4),
                            Winner = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                            is_Completed = reader.GetBoolean(6)
                        };
                    }
                    else
                    {
                        throw new AccsessEx("Battle could not be fetched");
                    }
                }
            }
            catch (NpgsqlException exception)
            {
                throw new DatabaseEx(exception.Message);
            }
            catch
            {
                throw;
            }
        }
        internal List<Battle> GetAvailableLobbies()
        {
            using var db = new DatabaseConnector();
            try
            {
                List<Battle> lobbies = new List<Battle>();
                using var command = db.CreateCommand("SELECT * FROM battles WHERE is_active = @bool_active AND is_completed = @bool_completed");
                db.AddParameter(command, "bool_active", DbType.Boolean, false);
                db.AddParameter(command, "bool_completed", DbType.Boolean, false);
                //locks
                using IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lobbies.Add(new Battle()
                    {
                        Id = reader.GetString(0),
                        User_id = reader.GetInt32(1),
                        Challenger_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                        is_Active = reader.GetBoolean(3),
                        is_Draw = reader.IsDBNull(4) ? null : reader.GetBoolean(4),
                        Winner = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                        is_Completed = reader.GetBoolean(6)
                    });
                }
                if (lobbies != null)
                {
                    return lobbies;
                }
                else
                {
                    throw new NotFoundEx("No lobbies available");
                }
            }
            catch (NpgsqlException exception)
            {
                throw new DatabaseEx(exception.Message);
            }
            catch
            {
                throw;
            }
        }
        internal bool JoinOpenLobby(string LobbyId, int ChallengerId)
        {
            using var db = new DatabaseConnector();
            try
            {
                //lock
                Battle? Lobby = GetLobbyById(LobbyId);
                if(!Lobby.is_Active && !Lobby.is_Completed)
                {
                    db.StartTransaction();
                    using var command = db.CreateCommand("UPDATE battles SET challenger_id = @challenger, is_active = @bool_active WHERE id = @lobby_id");
                    db.AddParameter(command, "lobby_id", DbType.String, LobbyId);
                    db.AddParameter(command, "bool_active", DbType.Boolean, true);
                    db.AddParameter(command, "challenger", DbType.Int32, ChallengerId);

                    if (command.ExecuteNonQuery() > 0)
                    {
                        db.CommitTransaction();
                        return true;
                    }
                    else
                    {
                        return false;
                    }    
                }
                else
                {
                    throw new AccsessEx("Lobby not available");
                }
            }
            catch (NpgsqlException exception)
            {
                db.RollbackTransaction();
                throw new DatabaseEx(exception.Message);
            }
            catch
            {
                db.RollbackTransaction();
                throw;
            }
        }
        //writes battle results into database
        internal void EvaluateBattle(string LobbyId, bool draw, int winner)
        {
            int attempts = 0;
            int maxAttempts = 3;
            using var db = new DatabaseConnector();
            
            while(attempts < maxAttempts)
            {
                try
                {
                    db.StartTransaction();
                    using var command = db.CreateCommand("UPDATE battles SET is_active = @bool_active, is_draw = @bool_draw, winner = @winner, is_completed = @bool_completed WHERE id = @id");
                    db.AddParameter(command, "bool_active", DbType.Boolean, false);
                    db.AddParameter(command, "bool_draw", DbType.Boolean, draw);
                    if (!draw) //if no draw write winner
                    {
                        db.AddParameter(command, "winner", DbType.Int32, winner);
                    }
                    else //draw winner is null
                    {
                        db.AddParameter(command, "winner", DbType.Int32, DBNull.Value);
                    }
                    db.AddParameter(command, "id", DbType.String, LobbyId);
                    db.AddParameter(command, "bool_completed", DbType.Boolean, true);
                    //lock
                    command.ExecuteNonQuery();
                    db.CommitTransaction();
                    break; //If successfull the loop is getting exitet
                }
                catch(NpgsqlException exception)
                {
                    db.RollbackTransaction();
                    attempts++;
                    if(attempts >= maxAttempts)
                    {
                        throw new DatabaseEx($"Database Update failed after {attempts} attempts. Error: {exception.Message}");
                    }
                }
                catch
                {
                    db.RollbackTransaction();
                    attempts++;
                    if (attempts >= maxAttempts)
                    {
                        throw new DatabaseEx("Failed to update database");
                    }
                }
            }
        }

        //List of open / available Lobby ids
        internal List<string> GetOpenLobbiesIds()
        {
            using var db = new DatabaseConnector();
            try
            {
                List<string> lobbieIds = new List<string>();
                using var command = db.CreateCommand("SELECT id FROM battles WHERE is_active = @bool_active AND is_completed = @bool_completed");
                db.AddParameter(command, "bool_active", DbType.Boolean, false);
                db.AddParameter(command, "bool_completed", DbType.Boolean, false);
                //locks
                using IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lobbieIds.Add(reader.GetString(0));
                  
                }
                if (lobbieIds != null)
                {
                    return lobbieIds;
                }
                else
                {
                    throw new NotFoundEx("No lobbies available");
                }
            }
            catch (NpgsqlException exception)
            {
                throw new DatabaseEx(exception.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
