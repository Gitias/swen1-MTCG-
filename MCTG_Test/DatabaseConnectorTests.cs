using MCTG.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Test
{
    public class DatabaseConnectorTests
    {
        [Test] //#5
        public void CommitTransaction_CommitsChangesToDatabase()
        {
            //Arrange
            using var db = new DatabaseConnector();
            var commandText_Insert = "INSERT INTO test (test) VALUES ('test_value')";
            var command_Insert = db.CreateCommand(commandText_Insert);
            var commandText_Select = "SELECT * FROM test";
            var command_Select = db.CreateCommand(commandText_Select);

            //Act
            db.StartTransaction();
            command_Insert.ExecuteNonQuery();
            db.CommitTransaction();

            //Assert
            using IDataReader reader = command_Select.ExecuteReader();
            if(reader.Read()) 
            {
                Assert.That(reader.GetString(1), Is.EqualTo("test_value"));
            }
        }
        [Test] //#6
        public void RollbackTransaction_DoesNotCommitChangesToDatabase()
        {
            //Arrange
            using var db = new DatabaseConnector();
            var commandText_Insert = "INSERT INTO test_rollback (test) VALUES ('test_value')";
            var command_Insert = db.CreateCommand(commandText_Insert);
            var commandText_Select = "SELECT * FROM test_rollback";
            var command_Select = db.CreateCommand(commandText_Select);
            var commandText_Delete = "DELETE FROM test_rollback WHERE test = 'test_value'";
            var command_Delete = db.CreateCommand(commandText_Delete);

            //Act
            db.StartTransaction();
            command_Insert.ExecuteNonQuery();
            db.CommitTransaction();

            db.StartTransaction();
            command_Delete.ExecuteNonQuery(); // should Delete
            db.RollbackTransaction(); // rololback deletion

            //Assert
            using IDataReader reader = command_Select.ExecuteReader();
            if (reader.Read())
            {
                Assert.That(reader.GetString(1), Is.EqualTo("test_value"));
            }
        }

        [Test] //# 7
        public void Dispose_Test_SchouldThrowException()
        {
            //Arrange
            using var db = new DatabaseConnector();

            //Act
            db.Dispose();

            //Assert
            Assert.Throws<ObjectDisposedException>(() => db.CreateCommand("SELECT * FROM test"));
        }
    }
}
