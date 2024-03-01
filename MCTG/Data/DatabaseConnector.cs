using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql;


namespace MCTG.Data
{
    internal class DatabaseConnector : IDisposable
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;
        public DatabaseConnector() 
        {
            _connection = new NpgsqlConnection("Host=localhost; Database=mctg; Username=postgres; Password=Snoopy2002; Persist Security Info=True");
            _connection.Open();
        }

        public IDbCommand CreateCommand(string commandText)
        {
            IDbCommand command = _connection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = _transaction; //Links the command with transaction
            return command;
        }

        public void AddParameter(IDbCommand command, string parameterName, DbType dbType, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
        public void StartTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if(_transaction != null)
            {
                _transaction.Commit(); //Commits transaction content
                _transaction = null; //Resets Transaction Content after commit was done
            }
        }

        public void RollbackTransaction()
        {
            if(_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null; //Resets Transaction Content after Rollback was done
            }
        }
        public void Dispose()
        {
            if (_transaction != null)
            {
                RollbackTransaction(); // Rollback any/every open transaction
            }
            _connection.Close();
            _connection.Dispose();
        }
    }
}
