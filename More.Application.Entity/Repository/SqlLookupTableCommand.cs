using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using More.Engine.Model;

namespace More.Application.Entity.Repository
{
    /// <summary>
    /// Implementation of an ITableContextCommand for SQL commands.
    /// </summary>
    public class SqlLookupTableCommand : ITableContextCommand
    {
        private readonly SqlCommand _command;
        private readonly LookupTableCommand _commandType;

        public SqlLookupTableCommand(SqlCommand command, LookupTableCommand commandType)
        {
            _command = command;
            _commandType = commandType;
        }

        /// <summary>
        /// Performs the work in executing the tasks for the given table context. Does not return any values.
        /// </summary>
        /// <param name="parameters">A listing of parameters to be used by the command.</param>
        public void Execute(params object[] parameters)
        {
            switch (_commandType)
            {
                case LookupTableCommand.SaveLookupTableKey:
                    ExecuteSaveLookupTableKeyCommand(parameters);
                    break;
                case LookupTableCommand.DeleteRow:
                    ExecuteDeleteRowCommand(parameters);
                    break;
                case LookupTableCommand.UpdateRow:
                    ExecuteUpdateRowCommand(parameters);
                    break;
                case LookupTableCommand.InsertRow:
                    ExecuteInsertRowCommand(parameters);
                    break;
                case LookupTableCommand.DeleteTable:
                case LookupTableCommand.DropTable:
                    ExecuteDropTableCommand(parameters);
                    break;
                case LookupTableCommand.CreateTable:
                    ExecuteCreateTableCommand(parameters);
                    break;
            }
        }

        /// <summary>
        /// Performs the work in executing the tasks for the given table context.
        /// </summary>
        /// <param name="parameters">A listing of parameters to be used by the command.</param>
        /// <returns>A DataTable representing the results of the execution.</returns>
        public DataTable ExecuteWithData(params object[] parameters)
        {
            switch (_commandType)
            {
                case LookupTableCommand.GetAllTables:
                    return ExecuteGetAllCommand(parameters);
                case LookupTableCommand.GetTable:
                    return ExecuteGetTableCommand(parameters.Select(p => (SqlParameter)p).ToArray());
                case LookupTableCommand.GetTableByName:
                    return ExecuteGetTableByNameCommand(parameters);
            }

            throw new Exception("Unknown command");
        }

        /// <summary>
        /// Executes the save lookup table key command. Parameters is a single Dictionary containing
        /// key/value pairs for the lookup table key.
        /// </summary>
        /// <param name="parameters">A list of values to use when updating the lookup table key.</param>
        private void ExecuteSaveLookupTableKeyCommand(params object[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute save lookup key command without any parameters.");

            var sqlParameters = (Dictionary<string, object>) parameters[0];

            using (_command)
            {
                _command.Connection.Open();

                _command.CommandText = FlatTableQueries.GetSqlUpdateExtendedPropertyColumnQuery(
                    sqlParameters["Schema"].ToString(),
                    sqlParameters["Table"].ToString(), 
                    sqlParameters["Property"].ToString(),
                    sqlParameters["Value"].ToString(), 
                    sqlParameters["Column"].ToString());

                _command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the drop table command. Parameters include a list of all table ids to drop, as well as
        /// an indicator as to whether the drop is hard or soft.
        /// </summary>
        /// <param name="parameters">A list of ids and a corresponding indicator of hard/soft delete.</param>
        private void ExecuteDropTableCommand(params object[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute DropTable command without at least one table id parameter.");

            using (_command)
            {
                _command.Connection.Open();

                foreach (var param in parameters)
                {
                    var sqlParameter = (Dictionary<string, object>)param;
                    _command.Parameters.AddRange(sqlParameter.Select(p => (SqlParameter)p.Value).ToArray());
                    _command.ExecuteNonQuery();
                    _command.Parameters.Clear();
                }
            }
        }

        /// <summary>
        /// Executes the create table command. Creates a table with extended properties as well as extended
        /// properties for each column. Takes a listing of table definitions.
        /// </summary>
        /// <param name="parameters">A listing of table definitions that represent tables to create.</param>
        private void ExecuteCreateTableCommand(params object[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute CreateTable command without any parameters.");

            //prepare the statements to execute
            var createTableList = new List<string>();

            foreach (var param in parameters)
            {
                var tableDefinitionList = (List<Dictionary<string, object>>)param;
                foreach (var tableDefinition in tableDefinitionList)
                {
                    var schema = (string) tableDefinition["Schema"];
                    var tableName = (string) tableDefinition["TableName"];
                    var changeId = (string) tableDefinition["ChangeId"];
                    var tableType = (string) tableDefinition["TableType"];
                    var keyColumns = (FactorTableColumnDefinition[]) tableDefinition["KeyColumns"];
                    var dataColumns = (FactorTableColumnDefinition[]) tableDefinition["DataColumns"];
                    var effectiveDate = (DateTime) tableDefinition["EffectiveDate"];

                    var metadataColumns = new[]
                    {
                        new FactorTableColumnDefinition("Id", false)
                        {
                            DataType = typeof (Int64)
                        },
                        new FactorTableColumnDefinition("Sequence", false)
                        {
                            DataType = typeof (Int32)
                        },
                        new FactorTableColumnDefinition("ChangeId", false)
                        {
                            DataType = typeof (Guid)
                        },
                        new FactorTableColumnDefinition("EffectiveDate", false)
                        {
                            DataType = typeof (DateTime)
                        },
                        new FactorTableColumnDefinition("ExpirationDate", false)
                        {
                            DataType = typeof (DateTime)
                        },
                        new FactorTableColumnDefinition("Active", false)
                        {
                            DataType = typeof (bool)
                        },
                        new FactorTableColumnDefinition("IsOverride", false)
                        {
                            DataType = typeof (bool)
                        }
                    };

                    createTableList.Add(
                        FlatTableQueries.GetCreateTableStatement(
                            schema, tableName, changeId, tableType, effectiveDate, metadataColumns, keyColumns, dataColumns));

                    //update the table metadata
                    var columns = new Dictionary<string, object>
                    {
                        { "TableName", tableName },
                        { "TableType", tableType },
                        { "ChangeId", changeId },
                        { "Schema", schema },
                        { "CreateDate", DateTime.Now },
                        { "EffectiveDate", effectiveDate },
                        { "Active", true }
                    };

                    createTableList.Add(
                        FlatTableQueries.GetSqlInsertQuery("dbo", "TableMetadata", columns));
                }
            }

            using (_command)
            {
                _command.Connection.Open();

                foreach (var createTableCommand in createTableList)
                {
                    _command.CommandText = createTableCommand;
                    _command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes the command to get all of the lookup tables (names/metadata, no rows). 
        /// The tables are retrieved based on the effective date. All tables are retrieved that 
        /// are less than or equal to the effective date.
        /// </summary>
        /// <param name="parameters">The effective date to be used for the retrieval.</param>
        /// <returns>A DataTable containing all of the tables based on the effective date.</returns>
        private DataTable ExecuteGetAllCommand(params object[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute GetAllTables command without the effective date parameter.");

            var table = new DataTable();

            var sqlParameter = new SqlParameter("EffectiveDate", SqlDbType.Date)
            {
                Value = (DateTime)parameters[0]
            };

            using (_command)
            {
                _command.Connection.Open();

                var adapter = new SqlDataAdapter
                {
                    SelectCommand = _command
                };

                adapter.SelectCommand.Parameters.Add(sqlParameter);

                adapter.Fill(table);

                return table;
            }
        }

        private DataTable ExecuteGetTableByNameCommand(params object[] parameters)
        {
            if(!parameters.Any())
                throw new ArgumentException("Cannot execute GetTableByName command without parameters.");

            var table = new DataTable();

            var sqlParameters = parameters.Where(p => p != null).Select(p => (SqlParameter) p).ToArray();

            using (_command)
            {
                try
                {
                    _command.Connection.Open();

                    var adapter = new SqlDataAdapter
                    {
                        SelectCommand = _command
                    };

                    adapter.SelectCommand.Parameters.AddRange(sqlParameters);
                    adapter.Fill(table);
                }
                catch (Exception)
                {
                    //TODO: do something meaningful here, like log it or something
                    throw;
                }
                finally
                {
                    _command.Parameters.Clear();
                }
            }

            return table;
        }

        /// <summary>
        /// Executes the command to get a single lookup table. Requires the id of the table to be 
        /// retrieved and the effective date. The table is retrieved with all of its metadata and rows.
        /// </summary>
        /// <param name="parameters">The effective date and the id of the table to retrieve.</param>
        /// <returns>A DataTable containing the table and its rows that correspond to the effective date/id supplied.</returns>
        private DataTable ExecuteGetTableCommand(params SqlParameter[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute GetTable command without parameters.");

            var table = new DataTable();

            using (_command)
            {
                _command.Connection.Open();
                

                var adapter = new SqlDataAdapter
                {
                    SelectCommand = _command
                };

                adapter.SelectCommand.Parameters.AddRange(parameters);
                adapter.Fill(table);
                _command.Parameters.Clear();

                return table;
            }
        }

        /// <summary>
        /// Executes the command to delete a row. Requires the id of the table, id of the row, and an indicator
        /// as to whether or not the delete is soft or hard.
        /// </summary>
        /// <param name="parameters">Id of the table, id of the row, and an indicator as to whether the delete is hard or soft.</param>
        private void ExecuteDeleteRowCommand(params object[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute DeleteRow command without parameters.");

            var queryParameters = parameters[0] as Dictionary<string, object>;

            if (queryParameters == null)
                throw new ArgumentException("A Dictionary of parameters is required for the DeleteRow command.");

            var setupMode = (bool)queryParameters["SetupMode"];
            var schema = queryParameters["Schema"].ToString();
            var table = queryParameters["Table"].ToString();
            var whereClause = (SqlWhereClauseItem)queryParameters["Where"];

            if (setupMode) //hard delete
            {
                _command.CommandText = FlatTableQueries.GetSqlDeleteRowQuery(schema, table, whereClause);
            }
            else
            {
                var updateColumns = (Dictionary<string, object>)queryParameters["UpdateColumns"];
                _command.CommandText = FlatTableQueries.GetSqlUpdateQuery(schema, table, updateColumns, whereClause);
            }

            using (_command)
            {
                _command.Connection.Open();
                _command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the command to update a row. Requires the id of the table as well as the columns/values to update
        /// and a list of where clause (SqlWhereClauseItem) conditions.
        /// </summary>
        /// <param name="parameters">Id of the table, columns/values to update, and a listing of SqlWhereClauseItems 
        /// that representing the where clause.</param>
        private void ExecuteUpdateRowCommand(params object[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute UpdateRow command without parameters.");

            var queryParameters = parameters[0] as Dictionary<string, object>;

            if (queryParameters == null)
                throw new ArgumentException("A Dictionary of parameters is required for the UpdateRow command.");

            _command.CommandText = FlatTableQueries.GetSqlUpdateQuery(queryParameters["Schema"].ToString(),
                queryParameters["Table"].ToString(),
                (Dictionary<string, object>)queryParameters["Values"],
                (SqlWhereClauseItem)queryParameters["Where"]);

            using (_command)
            {
                _command.Connection.Open();
                _command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the command to insert a row. Requires the id of the table as well as the columns/values to insert.
        /// </summary>
        /// <param name="parameters">Id of the table and the columns/values to update.</param>
        private void ExecuteInsertRowCommand(params object[] parameters)
        {
            if (!parameters.Any())
                throw new ArgumentException("Cannot execute InsertRow command without parameters.");

            var queryParameters = parameters[0] as Dictionary<string, object>;

            if (queryParameters == null)
                throw new ArgumentException("A Dictionary of parameters is required for the InsertRow command.");

            _command.CommandText = FlatTableQueries.GetSqlInsertQuery(queryParameters["Schema"].ToString(),
                queryParameters["Table"].ToString(),
                (Dictionary<string, object>)queryParameters["Values"]);

            using (_command)
            {
                _command.Connection.Open();
                _command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Disposes the connection maintained by the command.
        /// </summary>
        public void Dispose()
        {
            _command.Connection.Dispose();
        }
    }
}
