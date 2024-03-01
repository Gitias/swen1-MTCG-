using MCTG.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Data
{
    internal class UserRepository
    {
        internal void Add(Credentials user)
        {
            using var db = new DatabaseConnector();
            try
            {
                if (user.Username == null)
                {
                    throw new NotFoundEx("Username can not be null!");
                }
                else
                {
                    db.StartTransaction();
                    //must still be locked
                    using var command = db.CreateCommand(
                            "INSERT INTO users (username, password) " +
                            "VALUES (@username, @password)");
                    db.AddParameter(command, "username", DbType.String, user.Username);
                    db.AddParameter(command, "password", DbType.String, user.Password);

                    if (command.ExecuteNonQuery() <= 0) //executes command
                    {
                        throw new DatabaseEx("Could not create User");
                    }
                    db.CommitTransaction();
                    //----------------------------------------------- // Insert Id from new crated user in other Table
                    db.StartTransaction();
                    int newUserId = GetIdByUsername(user.Username);
                    Console.WriteLine("This is the ID of the newly crated user: " + newUserId);
                    command.CommandText = "INSERT INTO user_profile_data (name, bio, image, user_id) VALUES (null, null, null, @id)";
                    db.AddParameter(command, "id", DbType.Int32, newUserId);
                    //locks
                    command.ExecuteNonQuery(); //executes command

                    command.CommandText = "INSERT INTO user_stats(user_id, elo, wins, losses, coins, played, draws) VALUES(@user_id, @elo, @wins, @losses, @coins, @played, @draws)";

                    db.AddParameter(command, "user_id", DbType.Int32, newUserId);
                    db.AddParameter(command, "elo", DbType.Int32, 100); //starts with 100 elo points
                    db.AddParameter(command, "wins", DbType.Int32, 0);
                    db.AddParameter(command, "losses", DbType.Int32, 0);
                    db.AddParameter(command, "coins", DbType.Int32, 20); //PLayer start with 20 coins
                    db.AddParameter(command, "played", DbType.Int32, 0);
                    db.AddParameter(command, "draws", DbType.Int32, 0);
                    //locks
                    command.ExecuteNonQuery();

                    db.CommitTransaction();
                }
            }
            catch (NpgsqlException exception)
            {
                db.RollbackTransaction();
                if (exception.SqlState == "23505") //"23505" entspricht einem eindeutigen Verstoß in PostgreSQL, was bedeutet, dass ein Versuch, einen Datensatz einzufügen oder zu aktualisieren, aufgrund einer Verletzung einer Unique-Constrain
                {
                    throw new ExistingEx("At least one Unique Entry already exists");
                }
                else
                {
                    throw;
                }
            }
            catch
            {
                db.RollbackTransaction();
                throw;
            }


        }

        private int GetIdByUsername(string username)
        {
            using var db = new DatabaseConnector();
            try
            {
                if (username == null)
                {
                    throw new NotFoundEx("Username can not be found (null)");
                }
                else
                {
                    using var command = db.CreateCommand("SELECT id FROM users WHERE username = @username");
                    db.AddParameter(command, "username", DbType.String, username);
                    //lock
                    using IDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        throw new AccsessEx("User does not exist");
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
        //return whole User data
        internal User? GetUserById(int uId)
        {
            using var db = new DatabaseConnector();
            try
            {
                using var command = db.CreateCommand("SELECT users.id, users.username, user_profile_data.name, user_profile_data.bio, user_profile_data.image " +
                    "FROM users INNER JOIN user_profile_data ON users.id = user_profile_data.user_id " +
                    "WHERE users.id = @id;");
                User user;
                db.AddParameter(command, "id", DbType.Int32, uId);
                //locks
                using IDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    //initializing the User Object
                    user = new User()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Bio = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Image = reader.IsDBNull(4) ? null : reader.GetString(4)
                    };
                    return user;
                }
                return null;
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
        internal User? GetUserByUsername(string? username)
        {
            using var db = new DatabaseConnector();
            try
            {
                using var command = db.CreateCommand("SELECT users.id, users.username, user_profile_data.name, user_profile_data.bio, user_profile_data.image " +
                   "FROM users INNER JOIN user_profile_data ON users.id = user_profile_data.user_id " +
                   "WHERE users.username = @username;");
                User user;
                db.AddParameter(command, "username", DbType.String, username);
                //locks
                using IDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    //initializing the User Object
                    user = new User()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Bio = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Image = reader.IsDBNull(4) ? null : reader.GetString(4)
                    };
                    return user;
                }
                return null;

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

        internal List<User> GetAllUsers()
        {
            using var db = new DatabaseConnector();

            try
            {
                List<User> users = new();
                using var command = db.CreateCommand("SELECT users.id, users.username, user_profile_data.name, user_profile_data.bio, user_profile_data.image " +
                   "FROM users INNER JOIN user_profile_data ON users.id = user_profile_data.user_id " +
                   "WHERE users.username = @username;");
                //Locks
                using IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //Adds users to user list
                    users.Add(new User()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Bio = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Image = reader.IsDBNull(4) ? null : reader.GetString(4)
                    });
                }
                return users;

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

        internal void LoginUser(Credentials user)
        {
            using var db = new DatabaseConnector();

            try
            {
                //if user already exicsts Update "aktive"
                db.StartTransaction();
                using var command = db.CreateCommand("INSERT INTO sessions(token, is_active, user_id) VALUES (@token, @is_active, @user_id) " +
                    "ON CONFLICT(user_id) DO UPDATE " +
                    "SET is_active = @is_active WHERE sessions.user_id = @user_id;");

                var temporaryUser = GetCredentials(user.Username);
                if (temporaryUser.Password == user.Password)
                {
                    string token = "Bearer " + temporaryUser.Username + "-mtcgToken"; //Token based security based on Curl Script
                    db.AddParameter(command, "token", DbType.String, token);
                    db.AddParameter(command, "is_active", DbType.Boolean, true); //sets seesion on aktive
                    db.AddParameter(command, "user_id", DbType.Int32, temporaryUser.Id);
                    //locks
                    command.ExecuteNonQuery();

                    db.CommitTransaction();
                }
                else
                {
                    throw new UnauthorizedEx("Password is incorrect");
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

        private Credentials GetCredentials(string username)
        {
            using var db = new DatabaseConnector();

            try
            {
                using var command = db.CreateCommand("SELECT id, username, password FROM users WHERE username = @username;");
                db.AddParameter(command, "username", DbType.String, username);
                //locks
                using IDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Credentials()
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2)
                    };
                }
                else
                {
                    throw new AccsessEx("Unable to get user credentials");
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

        internal void LogOutuser(int uId)
        {
            using var db = new DatabaseConnector();

            try
            {
                db.StartTransaction();
                using var command = db.CreateCommand("UPDATE sessions SET is_active = @value WHERE user_id = @uId");
                db.AddParameter(command, "value", DbType.Boolean, false);
                db.AddParameter(command, "uId", DbType.Int32, uId);
                //lock
                if (command.ExecuteNonQuery() <= 0) //ExecuteNonQuery returrn 0 or -1 if no rows where impacted/edited
                {
                    throw new DatabaseEx("Unable to logout user");
                }
                db.CommitTransaction();
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

        //Node: Entering ä,ü,ö throws an error
        internal void Insert_Edit_UserProfile(int? Id, UserData userData)
        {
            using var db = new DatabaseConnector();
            try
            {
                db.StartTransaction();
                using var command = db.CreateCommand("INSERT INTO user_profile_data(name, bio, image, user_id) " +
                "VALUES (@name, @bio, @image, @user_id) " +
                "ON CONFLICT(user_id) DO UPDATE " +
                "SET name = @name, bio = @bio, image = @image WHERE user_profile_data.user_id = @user_id");
                db.AddParameter(command, "name", DbType.String, userData.Name);
                db.AddParameter(command, "bio", DbType.String, userData.Bio);
                db.AddParameter(command, "image", DbType.String, userData.Image);
                db.AddParameter(command, "user_id", DbType.Int32, Id);
                //lock
                command.ExecuteNonQuery();
                db.CommitTransaction();
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

        internal bool PayForPackage(int uId)
        {
            using var db = new DatabaseConnector();

            try
            {
                db.StartTransaction();
                using var command = db.CreateCommand("SELECT coins FROM user_stats WHERE user_id = @uId");
                db.AddParameter(command, "uid", DbType.Int32, uId);
                int coins = 0;
                //locks
                using IDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    coins = reader.GetInt32(0);
                    reader.Close();//when locks exist not needed anymore
                }
                else
                {
                    throw new DatabaseEx();
                }
                if (coins >= 5)
                {
                    coins -= 5;
                    command.CommandText = "UPDATE user_stats SET coins = @coins WHERE user_id = @uId";
                    db.AddParameter(command, "coins", DbType.Int32, coins);
                    db.AddParameter(command, "uId", DbType.Int32, uId);
                    //locks
                    command.ExecuteNonQuery();
                    db.CommitTransaction();
                    return true;
                }
                else
                {
                    throw new AccsessEx("Not enugh coins to purchase package");
                }
            }
            catch(NpgsqlException exception)
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

        internal UserStats GetUserStatsById(int? uId)
        {
            using var db = new DatabaseConnector();

            try
            {
                if (uId == null)
                {
                    throw new NotFoundEx("User id cant not be null!");
                }
                UserStats UserStats;
                using var command = db.CreateCommand("SELECT users.id, users.username, user_stats.elo, user_stats.wins, user_stats.losses, user_stats.coins, user_stats.played, user_stats.draws " +
                    "FROM users INNER JOIN user_stats ON users.id = user_stats.user_id " +
                    "WHERE users.id = @uId");
                db.AddParameter(command, "uId", DbType.Int32, uId);
                //locks
                //locks
                using IDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    UserStats = new UserStats()
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Elo = reader.GetInt32(2),
                        Wins = reader.GetInt32(3),
                        Losses = reader.GetInt32(4),
                        Coins = reader.GetInt32(5),
                        Played = reader.GetInt32(6),
                        Draws = reader.GetInt32(7)
                    };
                    return UserStats;
                }
                else
                {
                    throw new Exception("Error reading data");
                }
            }
            catch(NpgsqlException exception)
            {
                throw new DatabaseEx(exception.Message);
            }
            catch
            {
                throw;
            }
        }
        //Scoreboard
        internal List<UserStats> GetScoreboard()
        {
            using var db = new DatabaseConnector();
            try
            {
                List<UserStats> scoreboard = new();
                using var command = db.CreateCommand("SELECT users.username, user_stats.elo, user_stats.wins, user_stats.losses, user_stats.coins, user_stats.played, user_stats.draws " +
                    "FROM users INNER JOIN user_stats ON users.id = user_stats.user_id " +
                    "ORDER BY user_stats.elo DESC;");
                //locks
                //locks
                using IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    scoreboard.Add(new UserStats()
                    {
                        Username = reader.GetString(0),
                        Elo = reader.GetInt32(1),
                        Wins = reader.GetInt32(2),
                        Losses = reader.GetInt32(3),
                        Played = reader.GetInt32(4),
                        Draws = reader.GetInt32(5),
                    });
                }
                if(scoreboard.Count > 0)
                {
                    return scoreboard;
                }
                else
                {
                    throw new NotFoundEx("No values in Scoreboard");
                }
            }
            catch(NpgsqlException exception)
            {
                throw new DatabaseEx(exception.Message);
            }
            catch
            {
                throw;
            }
        }

        //Elo Calculations
        internal void UpdateElo(int winId, int loosId, bool draw)
        {
            int attempts = 0;
            using var db = new DatabaseConnector();

            while (attempts < 3) 
            {
                try
                {
                    UserStats winner_stats = GetUserStatsById(winId);
                    UserStats looser_sats = GetUserStatsById(loosId);
                    winner_stats.Played += 1; looser_sats.Played += 1;
                    if (draw)
                    {
                        winner_stats.Draws++; looser_sats.Draws++;
                        //Further calculation to balance unfair matchmaking
                        if (winner_stats.Elo > looser_sats.Elo)
                        {
                            looser_sats.Elo++;
                        }
                        else
                        {
                            winner_stats.Elo++;
                        }
                        winner_stats.Coins += 2; looser_sats.Coins += 2;
                    }
                    else
                    {
                        winner_stats.Wins++; looser_sats.Losses++;
                        winner_stats.Elo += 3;
                        if (looser_sats.Elo >= 5)
                        {
                            looser_sats.Elo -= 5;
                        }
                        else
                        {
                            looser_sats.Elo = 0;
                        }
                        winner_stats.Coins += 5;
                    }
                    db.StartTransaction();
                    using var command = db.CreateCommand("UPDATE user_stats SET elo = @elo, wins = @wins, coins = @coins, played = @played, draws = @draws WHERE user_id = @winnersId");
                    db.AddParameter(command, "elo", DbType.Int32, winner_stats.Elo);
                    db.AddParameter(command, "wins", DbType.Int32, winner_stats.Wins);
                    db.AddParameter(command, "coins", DbType.Int32, winner_stats.Coins);
                    db.AddParameter(command, "played", DbType.Int32, winner_stats.Played);
                    db.AddParameter(command, "draws", DbType.Int32, winner_stats.Draws);
                    db.AddParameter(command, "winnersId", DbType.Int32, winId);
                    //lock
                    command.ExecuteNonQuery();
                    command.Parameters.Clear(); //Clears added Parameters
                    command.CommandText = "UPDATE user_stats SET elo = @elo, losses = @losses, coins = @coins, played = @played, draws = @draws WHERE user_id = @loosersId";
                    db.AddParameter(command, "elo", DbType.Int32, looser_sats.Elo);
                    db.AddParameter(command, "losses", DbType.Int32, looser_sats.Losses);
                    db.AddParameter(command, "coins", DbType.Int32, looser_sats.Coins);
                    db.AddParameter(command, "played", DbType.Int32, looser_sats.Played);
                    db.AddParameter(command, "draws", DbType.Int32, looser_sats.Draws);
                    db.AddParameter(command, "loosersId", DbType.Int32, loosId);
                    //lock
                    command.ExecuteNonQuery();
                    db.CommitTransaction();
                    //leave loop if update was succesful
                    break;
                }
                catch(NpgsqlException exception)
                {
                    db.RollbackTransaction();
                    attempts++;
                    if (attempts == 3)
                    {
                        throw new DatabaseEx($"Updating user stats failed after {attempts} attemts");
                    }
                }
                catch
                {
                    db.RollbackTransaction();
                    attempts++;
                    if (attempts == 3)
                    {
                        throw new DatabaseEx($"Updating user stats failed after {attempts} attemts");
                    }
                }
            }
        }
    }
}

