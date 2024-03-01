using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MCTG.Server
{
    internal static class Authenticator
    {
        internal static (bool isTokenActive, int userId) AuthenticationCheck(string AuthToken)
        {
            try
            {
                using IDbConnection _connection = new NpgsqlConnection("Host=localhost; Database=mctg; Username=postgres; Password=Snoopy2002; Persist Security Info=True"); ;
                using IDbCommand command = _connection.CreateCommand();
                _connection.Open();
                command.CommandText = "SELECT is_active, user_id FROM sessions WHERE token = @token";
                AddParameter(command, "token", DbType.String, AuthToken);
                //locks
                using IDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetBoolean(0) == false)
                    {
                        throw new UnauthorizedEx("Access token is invalid (or not given)");
                    }
                    else
                    {
                        return (
                            reader.GetBoolean(0),
                            reader.GetInt32(1)
                        );
                    }

                }
                else
                {
                    throw new UnauthorizedEx("Unable to get authorization status");
                }
            }
            catch
            {
                throw;
            }
        }
        private static void AddParameter(IDbCommand command, string parameterName, DbType dbType, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}


