using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using More.Application.Entity.Repository;
using More.Engine.Model;

namespace More.Application.Entity
{
    public static class FlatTableQueries
    {
        public static string DeleteRowScript(string tableName, string rowId)
        {
            return string.Format("DELETE FROM [{0}] WHERE Id = {1}", tableName, rowId);
        }

        public static string InsertRowScript(string tableName, string[] columns, string[] values, string changeId, DateTime effectiveDate, bool active)
        {
            return string.Format(
                "INSERT INTO [{0}] ( [ChangeId],[EffectiveDate], [Active], {1} ) VALUES ( '{2}', '{3}', '{4}', {5} )",
                tableName,
                ColumnsFormated(columns),
                changeId,
                effectiveDate.ToString(CultureInfo.InvariantCulture),
                active ? "1" : "0",
                string.Join(",", values.Select(p => "'" + p + "'"))
                );
        }

        public static string UpdateRowScript(long rowId, string tableName, string[] columns, string[] values, string changeId, DateTime effectiveDate, bool active)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", tableName);
            sb.AppendFormat("ChangeId = '{0}', EffectiveDate = '{1}', Active = {2} ", changeId, effectiveDate.ToString(CultureInfo.InvariantCulture), active ? "1" : "0");
            for (int i = 0; i < columns.Length; i++)
            {
                sb.AppendFormat(", {0} = '{1}'", columns[i], values[i]);
            }
            sb.AppendFormat(" WHERE Id = {0}", rowId);
            return sb.ToString();
        }

        public static string SelectSingle(string table, string[] columns, string id)
        {
            return string.Format("SELECT [Id], [ChangeId], [EffectiveDate], [Active], {0} FROM [dbo].[{1}] where Id = {2}",
                                 ColumnsFormated(columns), table, string.IsNullOrEmpty(id) ? "0" : id);
        }

        public static string BuildVersionedQuery(string table, string[] columns, DateTime effectiveDate)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("SELECT [Id],[ChangeId], [EffectiveDate], [Active], {0} FROM [dbo].[{1}] ft ", ColumnsFormated(columns), table).AppendLine();
            sb.AppendLine("WHERE ft.[Id] IN (");
            sb.AppendFormat("SELECT MAX(ft1.[Id]) [Id] FROM [dbo].[{0}] ft1", table);
            sb.AppendLine(" WHERE ft1.[Id] IN (");
            sb.AppendLine("SELECT ft2.[Id] [Id]");
            sb.AppendFormat("FROM [dbo].[{0}] ft2 ", table);
            sb.AppendFormat("WHERE ft2.[EffectiveDate] <= '{0}'", effectiveDate.ToString());
            sb.AppendLine(")");
            sb.AppendLine("GROUP BY ft1.[ChangeId]");
            sb.AppendLine(")");

            return sb.ToString();
        }
        public static string UpdateColumnExtendedProperty(string tableName, string columnName, string name, string value)
        {
            return string.Format(
                "EXEC sys.sp_updateextendedproperty @name=N'{0}', @value=N'{1}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{2}', @level2type=N'COLUMN',@level2name=N'{3}'",
                    name, value, tableName, FixColumnName(columnName));
        }
        public static string FixColumnName(string name)
        {
            return name;
        }
        public static string BuildVersionedQueryCount(string table, DateTime effectiveDate)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("SELECT count(*) FROM [dbo].[{0}] ft ", table).AppendLine();
            sb.AppendLine("WHERE ft.[Id] IN (");
            sb.AppendFormat("SELECT MAX(ft1.[Id]) [Id] FROM [dbo].[{0}] ft1", table);
            sb.AppendLine(" WHERE ft1.[Id] IN (");
            sb.AppendLine("SELECT ft2.[Id] [Id]");
            sb.AppendFormat("FROM [dbo].[{0}] ft2 ", table);
            sb.AppendFormat("WHERE ft2.[EffectiveDate] <= '{0}'", effectiveDate.ToString());
            sb.AppendLine(")");
            sb.AppendLine("GROUP BY ft1.[ChangeId]");
            sb.AppendFormat(")").AppendLine();

            return sb.ToString();
        }

        private static string ColumnsFormated(IEnumerable<string> columns)
        {
            return string.Join(",", columns.Select(p => "[" + p + "]"));
        }

        /// <summary>
        /// Gets a formatted SQL query (targeted to SQL Server) for inserting rows into a table.
        /// </summary>
        /// <param name="schema">The schema of the database table.</param>
        /// <param name="table">The name of the table.</param>
        /// <param name="columnsToInsert">A listing of columns to insert.</param>
        /// <returns>The formatted SQL query.</returns>
        public static string GetSqlInsertQuery(string schema,
                                               string table,
                                               Dictionary<string, object> columnsToInsert)
        {
            if (!columnsToInsert.Any())
                throw new ArgumentException("Must specify at least one column/value to insert.");

            const string insertTemplate = "INSERT INTO [{0}].[{1}] ({2}) VALUES ({3});";

            var insertColumns = string.Join(",", columnsToInsert.Keys.Select(k => string.Format("[{0}]", k)));
            var insertValues = string.Join(",",
                columnsToInsert.Values.Select(v => string.Format("{0}", FormatSqlValue(v))));

            return string.Format(insertTemplate, schema, table, insertColumns, insertValues);
        }

        /// <summary>
        /// Gets a formatted SQL query (targeted to SQL Server) for updating rows in a table.
        /// </summary>
        /// <param name="schema">The schema of the database table.</param>
        /// <param name="table">The name of the database table.</param>
        /// <param name="columnsToUpdate">A listing of columns to update.</param>
        /// <param name="whereClause">A listing of where clause items used to restrict the update.</param>
        /// <returns>The formatted SQL query.</returns>
        public static string GetSqlUpdateQuery(string schema,
                                               string table,
                                               Dictionary<string, object> columnsToUpdate,
                                               params SqlWhereClauseItem[] whereClause)
        {
            if (!columnsToUpdate.Any())
                throw new ArgumentException("Must specify at least one column to update.");

            const string updateTemplate = "UPDATE [{0}].[{1}] SET {2}";

            var updateColumns = string.Join(",",
                columnsToUpdate.Select(c => string.Format("[{0}] = {1}", c.Key, FormatSqlValue(c.Value))));

            var query = new StringBuilder(string.Format(updateTemplate, schema, table, updateColumns));

            if (whereClause != null && whereClause.Any())
                return string.Format(query.Append(" WHERE {0};").ToString(),
                                     string.Join(",", whereClause.Select(i => i.ToString()).ToArray()));

            return query.ToString();
        }

        /// <summary>
        /// Gets a formatted SQL query (targeted to SQL Server) for deleting rows from a table.
        /// </summary>
        /// <param name="schema">The schema of the database table.</param>
        /// <param name="table">The name of the database table.</param>
        /// <param name="whereClause">A listing of where clause items used to restrict the update.</param>
        /// <returns>The formatted SQL query.</returns>
        public static string GetSqlDeleteRowQuery(string schema,
                                                  string table,
                                                  params SqlWhereClauseItem[] whereClause)
        {
            const string deleteTemplate = "DELETE FROM [{0}].[{1}]";

            var query = new StringBuilder(string.Format(deleteTemplate, schema, table));
            if (whereClause != null && whereClause.Any())
                return string.Format(query.Append(" WHERE {0};").ToString(),
                    string.Join(",", whereClause.Select(i => i.ToString()).ToArray()));

            return query.ToString();
        }

        /// <summary>
        /// Gets a formatted SQL query (targeted to SQL Server) for creating a table. The query will
        /// create the table and add extended properties to the table and the columns.
        /// </summary>
        /// <param name="schema">The name of the database schema.</param>
        /// <param name="table">The name of the table.</param>
        /// <param name="tableType"></param>
        /// <param name="effectiveDate">The effective date to add to the table.</param>
        /// <param name="metadataColumns">A listing of the metadata columns to apply to the table.</param>
        /// <param name="keyColumns">A listing of key columns to apply to the table.</param>
        /// <param name="dataColumns">A listing of data columns to apply to the table.</param>
        /// <param name="changeId"></param>
        /// <returns>The formatted SQL query.</returns>
        public static string GetCreateTableStatement(string schema,
                                                     string table,
                                                     string changeId,
                                                     string tableType,
                                                     DateTime effectiveDate,
                                                     FactorTableColumnDefinition[] metadataColumns,
                                                     FactorTableColumnDefinition[] keyColumns,
                                                     FactorTableColumnDefinition[] dataColumns)
        {
            if (string.IsNullOrEmpty(schema))
                throw new ArgumentNullException("schema");

            if (string.IsNullOrEmpty(table))
                throw new ArgumentNullException("table");

            if(string.IsNullOrEmpty(changeId))
                throw new ArgumentNullException("changeId");

            if (effectiveDate == DateTime.MinValue || effectiveDate == DateTime.MaxValue)
                throw new ArgumentException("Effective date must be a valid effective date.");

            if (!metadataColumns.Any() || !keyColumns.Any() || !dataColumns.Any())
                throw new ArgumentException("Metadata, key, and data columns must be supplied for table creation.");

            var allColumnNames =
                metadataColumns.Union(keyColumns)
                    .Union(dataColumns)
                    .Select(c => new { c.Name, c.DataType })
                    .ToDictionary(c => c.Name, c => c.DataType);

            const string createTemplate = "CREATE TABLE [{0}].[{1}] ({2});";

            //create table statement
            var builder = new StringBuilder();
            builder.AppendFormat(createTemplate, schema, table,
                string.Join(",", allColumnNames.Select(c => FormatCreateSqlColumn(c.Key, c.Value))))
                   .AppendLine();

            //table extended properties
            const string sqlTableExtendedPropertyFormat =
                "EXEC sp_addextendedproperty @name = N'{0}', @value = '{1}', @level0type = N'Schema', @level0name = '{2}', @level1type = 'Table', @level1name = '{3}';";

            builder.AppendFormat(sqlTableExtendedPropertyFormat, "Active", 1, schema, table).AppendLine();
            builder.AppendFormat(sqlTableExtendedPropertyFormat, "ChangeId", changeId, schema, table).AppendLine();
            builder.AppendFormat(sqlTableExtendedPropertyFormat, "CreateDate", DateTime.Now, schema, table).AppendLine();
            builder.AppendFormat(sqlTableExtendedPropertyFormat, "EffectiveDate", effectiveDate, schema, table).AppendLine();
            builder.AppendFormat(sqlTableExtendedPropertyFormat, "Id", 1, schema, table).AppendLine();
            builder.AppendFormat(sqlTableExtendedPropertyFormat, "TableType", tableType, schema, table).AppendLine();
            builder.AppendLine();

            //column extended properties
            const string sqlTableColumnExtendedPropertyFormat =
                "EXEC sp_addextendedproperty @name = N'{0}', @value = '{1}', @level0type = N'Schema', @level0name = '{2}', @level1type = 'Table', @level1name = '{3}', @level2type = 'Column', @level2name = '{4}';";

            var keysDataColumns = keyColumns.Union(dataColumns);

            foreach (var column in keysDataColumns)
            {
                builder.AppendFormat(sqlTableColumnExtendedPropertyFormat, "IsKey", column.IsKey ? 1 : 0, schema, table,
                    column.Name).AppendLine();

                if (column.IsKey)
                {
                    builder.AppendFormat(sqlTableColumnExtendedPropertyFormat, "LookupType",
                        Enum.GetName(typeof(LookupType), column.LookupType), schema, table, column.Name).AppendLine();
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a SQL formatted column using for creating a table. "Id" is treated as a special column (is
        /// an identity and primary key).
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="dataType">The .NET data type of the column to be converted into a SQL data type.</param>
        /// <param name="nullable">Flag indicating whether the column should be nullable.</param>
        /// <returns>The formatted SQL column for the create statement.</returns>
        public static string FormatCreateSqlColumn(string columnName, Type dataType, bool nullable = false)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");

            var sqlType = string.Empty;

            if (dataType == typeof(decimal) || dataType == typeof(float) || dataType == typeof(double))
                sqlType = "[decimal](19,5)";
            else if (dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(uint) || dataType == typeof(ushort))
                sqlType = "[int]";
            else if (dataType == typeof(long) || dataType == typeof(ulong))
                sqlType = "[bigint]";
            else if (dataType == typeof(bool))
                sqlType = "[bit]";
            else if (dataType == typeof(DateTime))
                sqlType = "[datetime]";
            else if (dataType == typeof(byte) || dataType == typeof(sbyte))
                sqlType = "[tinyint]";
            else if (dataType == typeof(byte[]))
                sqlType = "[varbinary](max)";
            else if (dataType == typeof(Guid))
                sqlType = "[uniqueidentifier]";
            else
                sqlType = "[nvarchar](1000)";

            var builder = new StringBuilder();
            builder.AppendFormat("[{0}] {1}", columnName, sqlType);

            if (columnName.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
            {
                if (nullable)
                    throw new ArgumentException("Column with name Id cannot be non-nullable");

                builder.Append(" IDENTITY(1,1) PRIMARY KEY");
                return builder.ToString();
            }

            builder.Append(nullable ? " NULL" : " NOT NULL");
            return builder.ToString();
        }

        /// <summary>
        /// Formats an individual that is appropriate for insert or update statements in SQL. Applies quotes
        /// where appropriate. The type of the object is used for this determination.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        /// <returns>The formatted SQL value.</returns>
        public static string FormatSqlValue(object value)
        {
            if (value == null)
                return string.Empty;

            return IsSqlStringy(value)
                ? string.Format("'{0}'", value)
                : string.Format("{0}", value is bool ? (bool)value ? "1" : "0" : value);
        }

        /// <summary>
        /// Determines whether a given value should be enclosed in quotes for inserting or updating
        /// in SQL. The type of the object is used for this determination.
        /// </summary>
        /// <param name="value">The value to be evaluted.</param>
        /// <returns>Bool indicating whether or not the value should be enclosed in quotes.</returns>
        public static bool IsSqlStringy(object value)
        {
            if (value == null)
                return false;

            var type = value.GetType();
            return type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(Guid) ||
                   type == typeof(DateTime?);
        }

        /// <summary>
        /// Gets a formatted SQL query (targeted to SQL server) for selecting all rows from a table.
        /// </summary>
        /// <param name="schema">The name of the database schema.</param>
        /// <param name="table">The name of the table.</param>
        /// <param name="columns">The column names that should be selected.</param>
        /// <param name="whereClause">A listing of where clause items used to restrict the update.</param>
        /// <returns>The formatted SQL query.</returns>
        public static string GetSqlSelectAllQuery(string schema,
                                                   string table,
                                                   IEnumerable<string> columns,
                                                   params SqlWhereClauseItem[] whereClause)
        {
            const string selectTemplate = "SELECT {0} from [{1}].[{2}]";

            var selectColumns = "*";

            if (columns != null)
            {
                var columnArray = columns as string[] ?? columns.ToArray();
                selectColumns = columnArray.Any()
                    ? string.Join(",", columnArray.Select(c => string.Format("[{0}]", c)))
                    : "*";
            }

            var query = new StringBuilder(string.Format(selectTemplate, selectColumns, schema, table));

            if (whereClause != null && whereClause.Any())
                return string.Format(query.Append(" WHERE {0};").ToString(),
                                     string.Join(",", whereClause.Select(i => i.ToString()).ToArray()));

            return query.ToString();
        }

        /// <summary>
        /// A convenience method for converting key/value pairs into SqlParameters.
        /// </summary>
        /// <param name="parameters">A listing of KeyValuePairs that need to be converted into SqlParameters.</param>
        /// <returns>An array of SqlParameters based on the KeyValuePair list.</returns>
        public static SqlParameter[] GetSqlParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return parameters.Select(p => new SqlParameter(string.Format("@{0}", p.Key), p.Value)).ToArray();
        }

        public static string GetSqlUpdateExtendedPropertyColumnQuery(string schema,
                                                                     string table,
                                                                     string property,
                                                                     string value,
                                                                     string columnName)
        {
            const string extendedPropertyTemplate =
                "EXEC sys.sp_updateextendedproperty @name=N'{0}', @value=N'{1}', @level0type=N'SCHEMA', @level0name=N'{2}', @level1type=N'TABLE' ,@level1name=N'{3}', @level2type=N'COLUMN',@level2name=N'{4}';";

            return string.Format(extendedPropertyTemplate, property, value, schema, table, columnName);
        }

        /// <summary>
        /// Converts a data row to a Dictionary.
        /// </summary>
        /// <param name="row">The data row to convert.</param>
        /// <param name="columnsToExclude">A list of columns names to exclude from the conversion.</param>
        /// <returns>A dictionary of key/value pairs that map to the column/row values for the data row.</returns>
        public static Dictionary<string, object> DataRowToDictionary(DataRow row,
                                                                      IEnumerable<string> columnsToExclude = null)
        {
            var dataRowValues = row.ItemArray.ToList();
            var dataRowColumns = row.Table.Columns.Cast<DataColumn>().ToList();

            if (columnsToExclude != null)
            {
                foreach (var column in columnsToExclude)
                {
                    var columnIndex = dataRowColumns.FindIndex(c => c.ColumnName == column);
                    dataRowValues.RemoveAt(columnIndex);
                    dataRowColumns.RemoveAt(columnIndex);
                }
            }

            return dataRowColumns.Zip(dataRowValues, (k, v) => new { k.ColumnName, v })
                                 .ToDictionary(x => x.ColumnName, x => x.v);
        }
    }
}