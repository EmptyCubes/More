using System;
using System.Data;
using System.Data.SqlClient;
using MongoDB.Driver;

namespace More.Application.Entity.Repository
{
    public class MongoLookupTableContext : ILookupTableContext
    {
        private readonly string _connectionString;

        public MongoLookupTableContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            _connectionString = connectionString;
        }

        public ITableContextCommand GetCommand(LookupTableCommand commandType)
        {
            var mongoClient = new MongoClient(_connectionString);
            var mongoServer = mongoClient.GetServer();
            var command = new MongoLookupTableCommand(mongoServer, mongoServer.GetDatabase("ISO"), commandType);

            switch (commandType)
            {
                case LookupTableCommand.SaveLookupTableKey:
                    //command.CommandType = CommandType.Text;
                    break;
                case LookupTableCommand.GetAllTables:
                    //command.CommandText = "GetAllTableMetadata";
                    //command.CommandType = CommandType.StoredProcedure;
                    break;
                case LookupTableCommand.GetTable:
                    //command.CommandText = "GetTableByChangeId";
                    //command.CommandType = CommandType.StoredProcedure;
                    break;
                case LookupTableCommand.DeleteRow:
                    //command.CommandType = CommandType.Text;
                    break;
                case LookupTableCommand.DropTable:
                    //command.CommandText = "DropTableByChangeId";
                    //command.CommandType = CommandType.StoredProcedure;
                    break;
                case LookupTableCommand.GetTableByName:
                    //command.CommandText = "Api.GetTables";
                    //command.CommandType = CommandType.StoredProcedure;
                    break;
            }

            return command;
        }
    }
}
