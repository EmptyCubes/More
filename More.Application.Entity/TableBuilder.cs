using System;
using System.Collections.Generic;
using System.Text;
using More.Engine.Model;
using System.Linq;
namespace More.Application.Entity
{
    public class TableBuilder
    {
        public bool DropTable { get; set; }

        public LookupTable Table { get; set; }

        public IEnumerable<string> GetAllScripts(int tableId, Guid changeId, DateTime tableEffectiveDate, bool active = true)
        {
            if (DropTable)
            {
                yield return DropTableScript();
            }
            yield return CreateTableScript();
            var extendedPropertyScripts = ExtendedPropertyScripts(tableId, changeId, tableEffectiveDate,
                                                                  active);
            var insertScripts = InsertScripts(tableEffectiveDate);

            foreach (var script in extendedPropertyScripts)
                yield return script;
            foreach (var script in insertScripts)
                yield return script;

        }

        public string DropTableScript()
        {

            return string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))  DROP TABLE [dbo].[{0}]", Table.Name);

        }

        public string CreateTableScript()
        {
            var script = new StringBuilder();
            script.AppendFormat(
                string.Format(
                    "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U')) BEGIN",
                    Table.Name)).AppendLine();
            script.AppendLine("CREATE TABLE " + Table.Name);
            script.AppendLine("(");
            script.AppendLine("\t Id BIGINT IDENTITY(1,1) NOT NULL,");
            script.AppendLine("\t [ChangeId] [uniqueidentifier] NOT NULL,");
            script.AppendLine("\t [EffectiveDate] [datetime] NOT NULL,");
            script.AppendLine("\t [Active] [bit] NOT NULL,");
            var keysAndColumns = Table.KeysAndColumns;
            for (int i = 0; i < keysAndColumns.Length; i++)
            {
                var field = keysAndColumns[i];


                script.Append("\t [" + field.Name + "] NVARCHAR(500) NOT NULL");


                if (i != keysAndColumns.Length - 1)
                {
                    script.Append(",");
                }

                script.Append(Environment.NewLine);
            }

            script.AppendLine(")");
            script.AppendLine("END");
            return script.ToString();
        }

        public IEnumerable<string> InsertScripts(DateTime effectiveDate)
        {
            foreach (var row in Table.Rows)
            {
                var values = row.KeyValues.Select(
                        p => p.LowValue + (string.IsNullOrEmpty(p.HighValue) ? string.Empty : "::" + p.HighValue)).
                        Concat(row.ColumnValues.Select(p => p.Value)).ToArray();

                yield return string.Format(
                    "INSERT INTO {0} ([{1}], [ChangeId], [EffectiveDate], [Active]) VALUES( '{2}','{3}','{4}', 1 )",
                    Table.Name,
                    string.Join("],[", Table.KeysAndColumns.Select(p => FlatTableQueries.FixColumnName(p.Name))),
                    string.Join("','", values),
                    Guid.NewGuid(),
                    effectiveDate);
            }

        }

        public IEnumerable<string> ExtendedPropertyScripts(int tableId, Guid changeId, DateTime tableEffectiveDate, bool active = true)
        {
            yield return TableExtendedProperty("Id", tableId);
            yield return TableExtendedProperty("CreateDate", DateTime.Now);
            yield return TableExtendedProperty("ChangeId", changeId.ToString());
            yield return TableExtendedProperty("EffectiveDate", tableEffectiveDate);
            yield return TableExtendedProperty("Active", active ? "1" : "0");

            foreach (var property in Table.TableProperties)
            {
                yield return TableExtendedProperty(property.Key, property.Value);
            }
            foreach (var key in this.Table.Keys)
            {
                yield return ColumnExtendedProperty(key, "LookupType", key.LookupType.ToString());
            }
            foreach (var column in this.Table.KeysAndColumns)
            {
                yield return ColumnExtendedProperty(column, "IsKey", column.IsKey ? "1" : "0");
                foreach (var property in column.Properties)
                    yield return ColumnExtendedProperty(column, property.Key, property.Value);
            }
        }

        private string ColumnExtendedProperty(FactorTableColumnDefinition column, string name, object value)
        {
            return string.Format(
                "EXEC sys.sp_addextendedproperty @name=N'{0}', @value=N'{1}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{2}', @level2type=N'COLUMN',@level2name=N'{3}'",
                    name, value, Table.Name, FlatTableQueries.FixColumnName(column.Name));
        }

        private string TableExtendedProperty(string name, object value)
        {
            return string.Format(
                "EXEC sys.sp_addextendedproperty @name = N'{0}', @value = N'{1}',  @level0type = N'SCHEMA', @level0name = 'DBO', @level1type = N'TABLE',  @level1name = '{2}';",
                name, value, Table.Name
                );
        }


    }

}