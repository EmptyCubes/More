using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using More.Application.BaseModel;
using More.Engine.Model;

namespace More.Application.Entity.Repository
{
    /// <summary>
    /// Implementation of the lookup table repository for interacting with a SQL Server database.
    /// </summary>
    public class MongoLookupTableRepository : IRatingEngineTableRepository
    {
        private readonly ILookupTableContext _context;
        protected DateTime EffectiveDate;
        protected bool SetupMode;

        private LookupTableAdapter _tableAdapter;

        protected LookupTableAdapter TableAdapter
        {
            get { return _tableAdapter ?? (_tableAdapter = new LookupTableAdapter()); }
        }

        public MongoLookupTableRepository(ILookupTableContext context, DateTime effectiveDate, bool setupMode)
        {
            _context = context;
            EffectiveDate = effectiveDate;
            SetupMode = setupMode;
        }

        /// <summary>
        /// Gets the lookup table view models based on the repository's effective date. The predicate
        /// is applied after the tables are retrieved.
        /// </summary>
        /// <param name="predicate">Predicate for filtering the lookup table models.</param>
        /// <returns>An enumeration of lookup table view models.</returns>
        public virtual IEnumerable<LookupTableModel> GetRatingTables(Func<LookupTableModel, bool> predicate)
        {
            var tables = GetRatingTables().Where(t => t.Properties.EffectiveDate <= EffectiveDate);

            return predicate != null ? tables.Where(predicate) : tables;
        }

        /// <summary>
        /// Gets an enumeration of lookup table view models based on the repository's effective date.
        /// </summary>
        /// <returns>An enumeration of lookup table view models.</returns>
        public virtual IEnumerable<LookupTableModel> GetRatingTables()
        {
            using (var command = _context.GetCommand(LookupTableCommand.GetAllTables))
            {
                var dataTable = command.ExecuteWithData(EffectiveDate);

                return dataTable.Rows.Cast<DataRow>().ToList().Select(TableAdapter.ToLookupTableModel);
            }
        }

        /// <summary>
        /// Gets a single lookup table view model based on the id of the table. The id is the Guid that correlates
        /// to the ChangeId of the table.
        /// </summary>
        /// <param name="id">Guid of the ChangeId of the table to be retrieved.</param>
        /// <returns>The lookup table view model. Null if nothing is found.</returns>
        public virtual LookupTableModel GetRatingTableModel(string id)
        {
            var table = GetLookupTable(id);

            return table != null ? TableAdapter.ToLookupTableModel(table) : null;
        }

        /// <summary>
        /// Saves a lookup table row for a lookup table. If the table doesn't exist, nothing occurs. If the row exists,
        /// it is updated; otherwise, the row is inserted.
        /// </summary>
        /// <param name="model">The lookup table row model to be updated.</param>
        public virtual void SaveLookupTableRow(LookupTableRowModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var lookupTable = GetLookupTable(model.TableId);
            if (lookupTable == null)
                return;

            //sync the column names
            var columns = LookupTableAdapter.MetadataColumnNames.Union(lookupTable.KeysAndColumns.Select(c => c.Name)).ToArray();
            var row = lookupTable.Rows.FirstOrDefault(r => Convert.ToInt64(r.RowId) == model.RowId && r.EffectiveDate == EffectiveDate);

            //get the columns/values
            var values = columns.Zip(model.Values, (col, val) => new { col, val })
                                    .ToDictionary(x => x.col, x =>
                                        TypeDescriptor.GetConverter(
                                            lookupTable.AllColumns.First(c => c.Name == x.col).DataType).ConvertFromString(x.val));

            //remove columns that don't need to be updated
            if (values.ContainsKey("Id"))
                values.Remove("Id");

            var parameters = new Dictionary<string, object>
            {
                { "Schema", lookupTable.Properties.Schema },
                { "Table", lookupTable.Name }
            };

            LookupTableCommand commandType;

            if (row == null) //add the row
            {
                commandType = LookupTableCommand.InsertRow;

                if (values.Count() != columns.Count() - 1)
                    throw new Exception(
                        string.Format("LookupTableRowModel value count ({0}) does not equal the number of columns in target table {1}.",
                            values.Count() + 1, lookupTable.Name));

                //set the changeId if it hasn't been set
                Guid changeId;
                if (Guid.TryParse(values["ChangeId"].ToString(), out changeId))
                {
                    if (changeId == Guid.Empty)
                        values["ChangeId"] = Guid.NewGuid();
                }
                else
                {
                    values["ChangeId"] = Guid.NewGuid();
                }

                //increment the sequence number if it's a low value
                if (Convert.ToInt64(values["Sequence"]) <= 0)
                    values["Sequence"] = lookupTable.Rows.Any() ? lookupTable.Rows.Max(r => r.Sequence) + 1 : 1;
            }
            else //update the row
            {
                commandType = LookupTableCommand.UpdateRow;

                parameters.Add("Where", new SqlWhereClauseItem
                {
                    ColumnName = "Id",
                    ColumnValue = model.RowId,
                    Operator = SqlWhereOperator.Equal
                });
            }

            parameters.Add("Values", values);

            using (var command = _context.GetCommand(commandType))
            {
                command.Execute(parameters);
            }
        }

        /// <summary>
        /// Retrieves the lookup table row view model for a given table id and row id. The table id correlates to the ChangeId
        /// of the table, the row id correlates to the Id column of the table.
        /// </summary>
        /// <param name="lookupTableName">Name of the table: not used for retrieval.</param>
        /// <param name="lookupTableId">Guid that correlates to the ChangeId of the table.</param>
        /// <param name="rowId">Long representing the id of the row to be retrieved.</param>
        /// <returns>The lookup table row view model; returns null if nothing is found.</returns>
        public virtual LookupTableRowModel GetLookupTableRow(string lookupTableName, string lookupTableId, string rowId)
        {
            if (string.IsNullOrEmpty(lookupTableId))
                throw new ArgumentNullException("lookupTableId");

            if (string.IsNullOrEmpty(rowId))
                throw new ArgumentNullException("rowId");

            int tableRowId;

            if (!int.TryParse(rowId, out tableRowId))
                throw new ArgumentException("rowId must be an integer.");

            var lookupTableModel = GetLookupTable(lookupTableId);
            if (lookupTableModel == null)
                return null;

            return TableAdapter.ToLookupTableRowModel(lookupTableModel,
                lookupTableModel.Rows.FirstOrDefault(r => Convert.ToInt64(r.RowId) == tableRowId));
        }

        /// <summary>
        /// Deletes a row from a lookup table. If SetupMode is true, the row is hard deleted. An inactive row is inserted
        /// when the effective date of the row differs from that of the repository; otherwise, the row is deleted
        /// appropriately.
        /// </summary>
        /// <param name="tableName">The name of the table; not used for retrieval.</param>
        /// <param name="tableId">Guid that correlates to the ChangeId of the table.</param>
        /// <param name="rowId">Long representing the id of the row to be deleted.</param>
        public virtual void DeleteLookupTableRow(string tableName, string tableId, string rowId)
        {
            if (string.IsNullOrEmpty(tableId))
                throw new ArgumentNullException("tableId");

            if (string.IsNullOrEmpty(rowId))
                throw new ArgumentNullException("rowId");

            int tableRowId;

            if (!int.TryParse(rowId, out tableRowId))
                throw new ArgumentException("rowId must be an integer.");

            var lookupTable = GetLookupTable(tableId);
            if (lookupTable == null)
                return;

            //if the row to delete has a different effective date than the instance, insert instead
            var row =
                lookupTable.Rows.FirstOrDefault(
                    r => Convert.ToInt64(r.RowId) == tableRowId && r.EffectiveDate == EffectiveDate);

            var parameters = new Dictionary<string, object>
            {
                { "Schema", lookupTable.Properties.Schema },
                { "Table", lookupTable.Name }
            };

            if (row != null)
            {
                //if setupmode is true, hard delete
                parameters.Add("SetupMode", SetupMode);
                parameters.Add("UpdateColumns", new Dictionary<string, object>
                {
                    {"Active", 0}
                });

                parameters.Add("Where", new SqlWhereClauseItem
                {
                    ColumnName = "Id",
                    ColumnValue = rowId,
                    Operator = SqlWhereOperator.Equal
                });

                using (var command = _context.GetCommand(LookupTableCommand.DeleteRow))
                {
                    command.Execute(parameters);
                }
            }
            else //insert it with the different effective date (max expiration date)
            {
                //TODO: refactor
                var values = new Dictionary<string, object>
                {
                    { "Sequence", 0 },
                    { "ChangeId", Guid.NewGuid().ToString() },
                    { "EffectiveDate", EffectiveDate },
                    { "ExpirationDate", DateTime.MaxValue },
                    { "Active", false },
                    { "IsOverride", true }
                };

                foreach (var key in lookupTable.Keys)
                {
                    object defaultValue;

                    if (key.DataType == typeof(string))
                        defaultValue = string.Empty;
                    else if (key.DataType == typeof(DateTime))
                        defaultValue = DateTime.MaxValue;
                    else
                        defaultValue = 0;

                    values.Add(key.Name, defaultValue);
                }

                foreach (var col in lookupTable.Columns)
                {
                    object defaultValue;

                    if (col.DataType == typeof(string))
                        defaultValue = string.Empty;
                    else if (col.DataType == typeof(DateTime))
                        defaultValue = DateTime.MaxValue;
                    else
                        defaultValue = 0;

                    values.Add(col.Name, defaultValue);
                }

                //increment the sequence number if it's a low value
                if (Convert.ToInt64(values["Sequence"]) <= 0)
                    values["Sequence"] = lookupTable.Rows.Max(r => r.Sequence) + 1;

                parameters.Add("Values", values);

                using (var command = _context.GetCommand(LookupTableCommand.InsertRow))
                {
                    command.Execute(parameters);
                }
            }
        }

        /// <summary>
        /// Gets the lookup table for the given id. The id correlates to the Guid (ChangeId) of the table. Includes the
        /// table's metadata and rows. Rows returned are restricted by the effective date of the repository.
        /// </summary>
        /// <param name="id">Guid that correlates to the ChangeId of the table.</param>
        /// <returns>The lookup table and rows correlating to the id and the effective date.</returns>
        public virtual LookupTable GetLookupTable(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            var metadataParameter = new SqlParameter("@Metadata", SqlDbType.NVarChar, 8000)
            {
                Direction = ParameterDirection.Output
            };

            var changeIdParameter = new SqlParameter("@Id", id);
            var effectiveDateParameter = new SqlParameter("@EffectiveDateIn", EffectiveDate);

            using (var command = _context.GetCommand(LookupTableCommand.GetTable))
            {
                var table = command.ExecuteWithData(metadataParameter, changeIdParameter, effectiveDateParameter);

                var parameterValues = new Dictionary<string, object>
                    {
                        { "Metadata", metadataParameter.Value },
                        { "ChangeId", changeIdParameter.Value },
                        { "EffectiveDate", effectiveDateParameter.Value }
                    };

                return TableAdapter.ToLookupTable(parameterValues, table);
            }
        }

        /// <summary>
        /// Creates a set of tables. If the drop flag is true and SetupMode is false, the tables are only soft deleted
        /// and are not recreated. Tables are dropped and then created with the appropriate metadata.
        /// </summary>
        /// <param name="importTables">The tables to create/drop.</param>
        /// <param name="drop">Indicates whether to drop a table. if SetupMode is true, the tables are physically dropped;
        /// otherwise, they are soft deleted.</param>
        public virtual void ImportTables(IEnumerable<LookupTable> importTables, bool drop)
        {
            if (importTables == null)
                throw new ArgumentNullException("importTables");

            var tables = importTables as LookupTable[] ?? importTables.ToArray();

            if (!tables.Any())
                return;

            //physically drop the tables if drop AND setupmode is true; otherwise, soft drop
            if (drop)
            {
                using (var command = _context.GetCommand(LookupTableCommand.DeleteTable))
                {
                    var parameters = tables.Where(t => !string.IsNullOrEmpty(t.Id)).Select(t => (object)
                        new Dictionary<string, object>
                        {
                            {"Id", new SqlParameter("@Id", t.Id)},
                            {"Permanent", new SqlParameter(@"Permanent", SetupMode)}
                        }).ToArray();

                    command.Execute(parameters);
                }
            }

            using (var command = _context.GetCommand(LookupTableCommand.CreateTable))
            {
                var parameters = new List<Dictionary<string, object>>();

                foreach (var table in tables)
                {
                    var tableProperties = new Dictionary<string, object>
                    {
                        { "EffectiveDate", EffectiveDate },
                        { "Schema", table.Properties.Schema },
                        { "TableName", table.Name },
                        { "KeyColumns", table.Keys },
                        { "DataColumns", table.Columns }
                    };

                    parameters.Add(tableProperties);
                }

                command.Execute(parameters);
            }
        }

        public void DropTables(IEnumerable<LookupTable> dropTables)
        {
            
        }

        /// <summary>
        /// Returns the lookup table key view model for a given table and column name.
        /// </summary>
        /// <param name="tableName">The name of the table (not used).</param>
        /// <param name="tableId">The id of the table. This is a string representation of a Guid.</param>
        /// <param name="tableColumn">The name of the column.</param>
        /// <returns>The lookup table key model for the given table/column. Null if not found.</returns>
        public virtual LookupTableKeyModel GetLookupTableKey(string tableName, string tableId, string tableColumn)
        {
            if(string.IsNullOrEmpty(tableId))
                throw new ArgumentNullException("tableId");

            if(string.IsNullOrEmpty(tableColumn))
                throw new ArgumentNullException("tableColumn");

            var table = GetLookupTable(tableId);

            var lookupTableKey = table.Keys.First(k => String.Equals(k.Name, tableColumn, StringComparison.CurrentCultureIgnoreCase));

            return TableAdapter.ToLookupTableKeyModel(lookupTableKey, table.Name, table.Id, table.Properties.Schema);
        }

        public virtual void SaveLookupTableKey(LookupTableKeyModel model)
        {
            if(model == null)
                throw new ArgumentNullException("model");

            var parameters = new Dictionary<string, object>();
            parameters["Table"] = model.TableName;
            parameters["Schema"] = model.TableSchema;
            parameters["Property"] = "LookupType";
            parameters["Value"] = model.LookupType.ToString();
            parameters["Column"] = model.Name;

            using (var command = _context.GetCommand(LookupTableCommand.SaveLookupTableKey))
            {
                command.Execute(parameters);
            }
        }

        public LookupTable GetLookupTable(string name, string schema, DateTime? effectiveDate)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if(string.IsNullOrEmpty(schema))
                throw new ArgumentNullException("schema"); 

            var tableNameParameter = new SqlParameter("TableName", name);
            var schemaNameParameter = new SqlParameter("Schema", schema);
            var metadataParameter = new SqlParameter("@Metadata", SqlDbType.NVarChar, 8000)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter effectiveDateParameter = null;

            if (effectiveDate.HasValue)
                effectiveDateParameter = new SqlParameter("EffectiveDateIn", effectiveDate.Value);

            using (var command = _context.GetCommand(LookupTableCommand.GetTableByName))
            {
                var table = command.ExecuteWithData(tableNameParameter, schemaNameParameter, effectiveDateParameter, metadataParameter);

                return TableAdapter.ToLookupTable(new Dictionary<string, object>
                {
                    { "Metadata", metadataParameter.Value },
                    { "ChangeId", string.Empty }
                }, table);
            }
        }

        public void ChangeEffectiveDate(DateTime effectiveDate)
        {
            EffectiveDate = effectiveDate;
        }

        #region NotImplemented
        public virtual void SaveRatingTableModel(LookupTableModel model)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteRateTableColumn(int id)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteLookupTableKey(int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
