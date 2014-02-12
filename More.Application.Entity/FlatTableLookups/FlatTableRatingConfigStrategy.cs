using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using More.Application.BaseModel;
using More.Engine.BaseModel;
using More.Engine.Model;

namespace More.Application.Entity
{
    public class FlatTableRatingConfigStrategy : ILookupTablesRepository
    {
        public DateTime EffectiveDate { get; set; }

        public MoreLookupTableEntities RatingTablesDb { get; set; }

        public bool SetupMode { get; set; }

        public FlatTableRatingConfigStrategy(DateTime effectiveDate, MoreLookupTableEntities lookupDb)
        {
            RatingTablesDb = lookupDb;
            EffectiveDate = effectiveDate;
        }

        public IEnumerable<LookupTable> DecorateTables(bool definitionOnly, string[] tableNames = null)
        {
            var tables = RatingTablesDb.GetAllFactorTables().ToArray();
            var allColumns = RatingTablesDb.GetTableColumns().ToArray();
            foreach (var table in tables)
            {
                if (tableNames != null && !tableNames.Contains(table.TABLE_NAME)) continue;
                var tableColumns = allColumns.Where(p => p.TableName.ToUpper() == table.TABLE_NAME.ToUpper()).ToArray();
                //var flatTable =
                yield return DecorateFlatTable(table.TABLE_NAME, tableColumns, !definitionOnly);
                //if (flatTable.Rows.Count > 0)
                //{
                //    yield return flatTable;
                //}
            }
        }

        public void DeleteLookupTableRow(string tableName, string tableId, string rowId)
        {
            var row = GetLookupTableRow(tableName, tableId, rowId);
            if (SetupMode || row.EffectiveDate == EffectiveDate)
            {
                this.RatingTableQuery(FlatTableQueries.DeleteRowScript(tableName, rowId));
            }
            else
            {
                row.RowId = 0;
                row.EffectiveDate = EffectiveDate;
                row.Active = false;
                SaveLookupTableRow(row);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> GetItems(string tableName, string textColumn, string valueColumn)
        {
            var items = new Dictionary<string, string>();

            RatingTableQuery(string.Format("SELECT DISTINCT [{0}],[{1}] from [{2}] ", textColumn, valueColumn, tableName), r =>
                                                                                                 {
                                                                                                     while (r.Read())
                                                                                                     {
                                                                                                         var text =
                                                                                                             r.GetString
                                                                                                                 (0);
                                                                                                         var value =
                                                                                                             r.GetString
                                                                                                                 (1);
                                                                                                         items.Add(text, value);
                                                                                                     }
                                                                                                 });
            return items;
        }

        public void GetKeyProperties(string tableName, string columnName)
        {
        }

        public LookupTable GetLookupTable(string id)
        {
            var intId = Convert.ToInt32(id);
            var table = RatingTablesDb.GetFactorTables(EffectiveDate).FirstOrDefault(p => p.Id == intId);
            if (table == null) return null;
            var tbl = DecorateFlatTable(table.TableName, GetTableColumns(table.TableName));
            tbl.Id = id;
            return tbl;
        }

        public LookupTableKeyModel GetLookupTableKey(string tableName, string tableId, string tableColumn)
        {
            var column = GetTableColumns(tableName).FirstOrDefault(p => p.TableName.ToUpper() == tableName.ToUpper());
            return new LookupTableKeyModel()
            {
                TableName = tableName,
                TableId = tableId,
                Name = column.ColumnName,
                LookupType = (LookupType)Enum.Parse(typeof(LookupType), column.LookupType, true)
            };
        }

        public LookupTableRowModel GetLookupTableRow(string tableName, string tableId, string rowId)
        {
            var LookupTableRow = new LookupTableRowModel() { Active = true, ChangeId = Guid.NewGuid(), TableId = tableId, EffectiveDate = EffectiveDate };
            var tableColumns = GetTableColumns(tableName);
            var values = new string[tableColumns.Length];

            RatingTableQuery(FlatTableQueries.SelectSingle(tableName,
                                                           GetTableColumns(tableName).Select(p => p.ColumnName).ToArray(),
                                                           rowId),
                                                           reader =>
                                                           {
                                                               if (reader.HasRows)
                                                               {
                                                                   reader.Read(); // Read this first row
                                                                   var index = 0;
                                                                   LookupTableRow.RowId =
                                                                       reader.GetInt64(index++); //.ToString();
                                                                   LookupTableRow.ChangeId = reader.GetGuid(index++);
                                                                   LookupTableRow.EffectiveDate =
                                                                       reader.GetDateTime(index++);
                                                                   //LookupTableRow.ExceptionId = reader.GetInt32(index++);
                                                                   LookupTableRow.Active = reader.GetBoolean(index++);

                                                                   for (var i = 0;
                                                                        i < (reader.FieldCount - 4);
                                                                        i++)
                                                                       values[i] = reader.GetString(index++);

                                                                   LookupTableRow.Values = values.ToArray();
                                                               }
                                                           });
            LookupTableRow.TableName = tableName;
            LookupTableRow.Values = values;
            return LookupTableRow;
        }

        public LookupTableModel GetRatingTableModel(string id)
        {
            return GetRatingTables().FirstOrDefault(p => p.Name.ToUpper() == id.ToUpper());
        }

        public IEnumerable<LookupTableModel> GetRatingTables()
        {
            var tables = RatingTablesDb.GetFactorTables(EffectiveDate);
            return tables.Select(p => new LookupTableModel()
                                          {
                                              Active = true,
                                              ChangeId = null,
                                              Id = p.Id.ToString(),
                                              Name = p.TableName
                                          }).ToArray();
        }

        public void ImportTables(IEnumerable<LookupTable> importTables, bool drop)
        {
            var id = GetNextId();
            foreach (var importTable in importTables)
            {
                var builder = new TableBuilder() { Table = importTable, DropTable = drop };
                var scripts = builder.GetAllScripts(id, Guid.NewGuid(), DateTime.Now);
                try
                {
                    RatingTablesDb.ExecuteBatchNonQuery(scripts.ToArray());
                }
                catch (Exception ex)
                {
                    // TODO add to a result object
                }
                id++;
            }
        }

        public void RatingTableQuery(string query, Action<SqlDataReader> read)
        {
            using (var connection = new SqlConnection((RatingTablesDb.Connection as EntityConnection).StoreConnection.ConnectionString + ";password=xpress"))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    read(reader);
                }
                connection.Close();
            }
        }

        public void RatingTableQuery(string query)
        {
            using (var connection = new SqlConnection((RatingTablesDb.Connection as EntityConnection).StoreConnection.ConnectionString + ";password=xpress"))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void SaveLookupTableKey(LookupTableKeyModel model)
        {
            RatingTableQuery(FlatTableQueries.UpdateColumnExtendedProperty(model.TableName, model.Name, "LookupType", model.LookupType.ToString()));
        }

        public void SaveLookupTableRow(LookupTableRowModel model)
        {
            var columns = GetTableColumns(model.TableName);
            var columnNames = columns.Select(p => p.ColumnName).ToArray();

            //if (!SetupMode)
            //{
            //(string.IsNullOrEmpty(model.RowId) || model.RowId == "0")
            if (model.RowId == 0 || model.EffectiveDate != EffectiveDate)
            {
                // Create new row
                RatingTableQuery(FlatTableQueries.InsertRowScript(
                    model.TableName, columnNames, model.Values, model.ChangeId.ToString(), EffectiveDate, model.Active));
            }
            else if (model.EffectiveDate == EffectiveDate)
            {
                // Update the row
                RatingTableQuery(FlatTableQueries.UpdateRowScript(model.RowId, model.TableName, columnNames, model.Values, model.ChangeId.ToString(), model.EffectiveDate, model.Active));
            }

            //}
            //else
            //{
            //            }
        }

        public void SaveRatingTableModel(LookupTableModel model)
        {
        }

        private static void ReadLookupTableRows(LookupTable ratingTable, SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var LookupTableRow = new LookupTableRow();
                    var index = 0;
                    LookupTableRow.RowId = reader.GetInt64(index++).ToString();
                    LookupTableRow.ChangeId = reader.GetGuid(index++).ToString();
                    LookupTableRow.EffectiveDate = reader.GetDateTime(index++);
                    LookupTableRow.Active = reader.GetBoolean(index++);

                    foreach (var key in ratingTable.Keys)
                    {
                        var value = reader.GetString(index);
                        if (value.Contains("::"))
                        {
                            var lhValues = value.Split("::".ToCharArray(),
                                                       StringSplitOptions.RemoveEmptyEntries);
                            LookupTableRow.KeyValues.Add(new LookupTableKey(lhValues[0],
                                                                            lhValues[1]));
                        }
                        else
                        {
                            LookupTableRow.KeyValues.Add(new LookupTableKey(value, null));
                        }

                        index++;
                    }
                    foreach (var column in ratingTable.Columns)
                    {
                        LookupTableRow.ColumnValues.Add(column.Name, reader.GetString(index));
                        index++;
                    }
                    ratingTable.Rows.Add(LookupTableRow);
                }
            }
        }

        private LookupTable DecorateFlatTable(string name, GetTableColumns_Result[] tableColumns, bool includeRows = true)
        {
            var ratingTable = new LookupTable(name)
            {
                Keys =
                    tableColumns.Where(p => p.IsKey != null && p.IsKey == true).Select(
                        p => new FactorTableColumnDefinition(p.ColumnName, true)
                                 {
                                     LookupType = (LookupType)Enum.Parse(typeof(LookupType), p.LookupType, true)
                                 }).ToArray(),
                Columns =
                    tableColumns.Where(p => p.IsKey != null && p.IsKey == false).Select(
                        p => new FactorTableColumnDefinition(p.ColumnName, false)).ToArray()
            };
            if (includeRows)
            {
                var allColumns = ratingTable.Keys.Concat(ratingTable.Columns).ToArray();
                //RatingTablesDb.Connection.Open();

                RatingTableQuery(
                    FlatTableQueries.BuildVersionedQuery(ratingTable.Name, allColumns.Select(p => p.Name).ToArray(), EffectiveDate),
                        reader => ReadLookupTableRows(ratingTable, reader));
            }
            return ratingTable;
        }

        private int GetNextId()
        {
            try
            {
                return RatingTablesDb.GetNextFactorTableId().First().Column1 ?? 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private GetTableColumns_Result[] GetTableColumns(string tableName)
        {
            return RatingTablesDb.GetTableColumns().ToArray().Where(
                p => p.TableName.ToUpper() == tableName.ToUpper()).ToArray();
        }

        #region No Implementation

        public void DeleteLookupTableKey(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteRateTableColumn(int id)
        {
            throw new NotImplementedException();
        }

        //public void SaveRatingTableColumn(LookupTableColumnModel model)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion No Implementation
    }
}