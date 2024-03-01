using MCTG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace MCTG.Data
{
    internal class CardRepository
    {
        internal void Add(Package package, int uId)
        {
            using var db = new DatabaseConnector();
            try
            {
                using var command = db.CreateCommand("INSERT INTO cards (id, name, type, element, user_id, damage, in_deck, monster)" + "VALUES (@id, @name, @type, @element, @user_id, @damage, @in_deck, @monster)");
                foreach(Card card in package.cards)
                {
                    db.AddParameter(command, "id", DbType.String, card.Id);
                    db.AddParameter(command, "name", DbType.String, card.Name);
                    db.AddParameter(command, "type", DbType.String, card.Type.ToString());
                    db.AddParameter(command, "element", DbType.String, card.Element.ToString());
                    db.AddParameter(command, "user_id", DbType.Int32, uId);
                    db.AddParameter(command, "damage", DbType.Int32, card.Damage);
                    db.AddParameter(command, "in_deck", DbType.Boolean, false);
                    if(card.Monster != null)
                    {
                        db.AddParameter(command, "monster", DbType.String, card.Monster.ToString());
                    }
                    else
                    {
                        db.AddParameter(command, "monster", DbType.Boolean, DBNull.Value);
                    }
                    //locks
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
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
        internal Stack GetUserStack(int uId) 
        {
            using var db = new DatabaseConnector();
            try
            {
                Stack UserStack = new();
                UserStack.UserId = uId;
                using var command = db.CreateCommand("SELECT * FROM cards WHERE user_id = @uId");
                db.AddParameter(command, "uId", DbType.Int32, uId);
                //lock
                using IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Card card = new()
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Type = (CardType)Enum.Parse(typeof(CardType), reader.GetString(2), true),
                        Element = (ElementType)Enum.Parse(typeof(ElementType), reader.GetString(3), true),
                        UserId = reader.GetInt32(4),
                        Damage = reader.GetFloat(5),
                        InDeck = reader.GetBoolean(6),
                        Monster = reader.IsDBNull(7) ? (Monsters?)null : (Monsters)Enum.Parse(typeof(Monsters), reader.GetString(7), true)
                    };
                    UserStack.cards.Add(card);
                }
                return UserStack;
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
        internal List<Card> GetUserDeck(int uId)
        {
            using var db = new DatabaseConnector();
            try
            {
                List<Card> Deck = new();
                using var command = db.CreateCommand("SELECT * FROM cards WHERE user_id = @id AND in_deck = @value");
                db.AddParameter(command, "id", DbType.Int32, uId);
                db.AddParameter(command, "value", DbType.Boolean, true);
                //lock
                using IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Card card = new()
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Type = (CardType)Enum.Parse(typeof(CardType), reader.GetString(2), true),
                        Element = (ElementType)Enum.Parse(typeof(ElementType), reader.GetString(3), true),
                        UserId = reader.GetInt32(4),
                        Damage = reader.GetFloat(5),
                        InDeck = reader.GetBoolean(6),
                        Monster = reader.IsDBNull(7) ? null : (Monsters)Enum.Parse(typeof(Monsters), reader.GetString(7), true),
                    };
                    Deck.Add(card);
                }
                return Deck;
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
        internal void EditDeck(List<string> chosenCards, int uId)
        {
            using var db = new DatabaseConnector();
            try
            {
                int count = 0;
                db.StartTransaction();
                using var command = db.CreateCommand("SELECT * FROM cards WHERE id IN (@id_1, @id_2, @id_3, @id_4)");
                db.AddParameter(command, "id_1", DbType.String, chosenCards[0]);
                db.AddParameter(command, "id_2", DbType.String, chosenCards[1]);
                db.AddParameter(command, "id_3", DbType.String, chosenCards[2]);
                db.AddParameter(command, "id_4", DbType.String, chosenCards[3]);
                //locks
                using var reader = command.ExecuteReader();
                if(reader.Read())
                {
                    do
                    {
                        count++;
                        if (reader.GetInt32(4) != uId)
                        {
                            throw new AccsessEx("At least one card does not belong to the user");
                        }
                            
                    } while (reader.Read());
                    reader.Close(); //when locks exist not needed anymore
                }
                else
                {
                    throw new NotFoundEx("Unable to find matching cards");
                }
                if(count != 4)
                {
                    throw new BadRequEx("Selection must contain 4 Cards");
                }
              
                command.CommandText = "UPDATE cards SET in_deck = @value WHERE id IN (@id_1, @id_2, @id_3, @id_4)";
                db.AddParameter(command, "value", DbType.Boolean, true);
                db.AddParameter(command, "id_1", DbType.String, chosenCards[0]);
                db.AddParameter(command, "id_2", DbType.String, chosenCards[1]);
                db.AddParameter(command, "id_3", DbType.String, chosenCards[2]);
                db.AddParameter(command, "id_4", DbType.String, chosenCards[3]);
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
        internal Card? GetCardByID(string id)
        {
            using var db = new DatabaseConnector();
            try
            {
                if(id == null)
                {
                    throw new NotFoundEx("CardId has no value (not null)");
                }
                else
                {
                    using var command = db.CreateCommand("SELECT * FROM cards WHERE id = @cardId");
                    db.AddParameter(command, "cardId", DbType.String, id);
                    //lock
                    using IDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Card()
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            Type = (CardType)Enum.Parse(typeof(CardType), reader.GetString(2), true),
                            Element = (ElementType)Enum.Parse(typeof(ElementType), reader.GetString(3), true),
                            UserId = reader.GetInt32(4),
                            Damage = reader.GetFloat(5),
                            InDeck = reader.GetBoolean(6),
                            Monster = reader.IsDBNull(7) ? (Monsters?)null : (Monsters)Enum.Parse(typeof(Monsters), reader.GetString(7), true)
                        };
                    }
                    else
                        throw new AccsessEx("Unable to get Card");
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
        internal void ResetDeck(int uId)
        {
            using var db = new DatabaseConnector();
            try
            {
                db.StartTransaction(); // Beginnen einer Transaktion
                using var command = db.CreateCommand("UPDATE cards SET in_deck = @in_deck WHERE user_id = @user_id AND in_deck = TRUE");
                db.AddParameter(command, "in_deck", DbType.Boolean, false); // Setzen von in_deck auf false
                db.AddParameter(command, "user_id", DbType.Int32, uId); // Festlegen der Benutzer-ID

                command.ExecuteNonQuery(); // Ausführen des Befehls
                db.CommitTransaction(); // Commit der Transaktion, um die Änderungen zu bestätigen
            }
            catch (NpgsqlException exception)
            {
                db.RollbackTransaction(); // Rollback der Transaktion im Fehlerfall
                throw new DatabaseEx(exception.Message);
            }
            catch
            {
                db.RollbackTransaction(); // Sicherstellen, dass die Transaktion auch bei anderen Fehlern zurückgerollt wird
                throw;
            }
        }


    }
}
