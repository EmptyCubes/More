using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;
using More.Application.BaseModel;
using More.Engine.Model;

namespace More.Application.Entity.Repository
{
    public class LookupTableAdapter
    {
        public static readonly string[] MetadataColumnNames =
        {
            "Id",
            "Sequence",
            "ChangeId",
            "EffectiveDate",
            "ExpirationDate",
            "Active",
            "IsOverride"//,
            //"WritingCompany"
        };

        public LookupTableModel ToLookupTableModel(DataRow row)
        {
            //TODO: validate values prior to assignment
            var model = new LookupTableModel
            {
                Id = row["TableId"].ToString(),
                ChangeId = row["ChangeId"].ToString(),
                Name = row["TableName"].ToString(),
                Description = row["TableName"].ToString(),
                IsoName = row["TableName"].ToString(),
                Active = Convert.ToBoolean(row["Active"])
            };

            model.Properties.EffectiveDate = Convert.ToDateTime(row["EffectiveDate"]);
            model.Properties.Schema = row["Schema"].ToString();
            model.Properties.TableType = row["TableType"].ToString();

            return model;
        }

        public LookupTableModel ToLookupTableModel(LookupTable table)
        {
            var model = new LookupTableModel
            {
                Id = table.Id,
                ChangeId = table.Id,
                Name = table.Name,
                Description = table.Name,
                IsoName = table.Name,
                Active = table.Properties.Active == "1"
            };

            model.Properties.EffectiveDate = Convert.ToDateTime(table.Properties.EffectiveDate);
            model.Properties.Schema = Convert.ToString(table.Properties.Schema);
            model.Properties.TableType = Convert.ToString(table.Properties.TableType);
            model.Properties.Context = Convert.ToString(table.Properties.Context);

            return model;
        }

        public LookupTableRowModel ToLookupTableRowModel(string tableId, string tableName, DataRow row)
        {
            //TODO: validate values prior to assignment
            return new LookupTableRowModel
            {
                TableId = tableId,
                TableName = tableName,
                ItemValues = FlatTableQueries.DataRowToDictionary(row),
                Values = row.ItemArray.Select(i => i.ToString()).ToArray(),
                Active = Convert.ToBoolean(row["Active"].ToString()),
                EffectiveDate = Convert.ToDateTime(row["EffectiveDate"]),
                RowId = Convert.ToInt64(row["Id"]),
                ChangeId = Guid.Parse(row["ChangeId"].ToString()),
                Sequence = Convert.ToInt32(row["Sequence"].ToString()),
                IsOverride = Convert.ToBoolean(row["IsOverride"])
            };
        }

        public LookupTableKeyModel ToLookupTableKeyModel(FactorTableColumnDefinition keyColumn, string tableName, string tableId, string tableSchema)
        {
            return new LookupTableKeyModel
            {
                Name = keyColumn.Name,
                TableId = tableId,
                TableName = tableName,
                TableSchema = tableSchema,
                LookupType = keyColumn.LookupType
            };
        }

        public LookupTableRowModel ToLookupTableRowModel(LookupTable table, LookupTableRow row)
        {
            if (row == null)
                return null;

            var metadata = new Dictionary<string, object>
            {
                { "Id", row.RowId },
                { "Sequence", row.Sequence },
                { "ChangeId", row.ChangeId },
                { "EffectiveDate", row.EffectiveDate },
                { "ExpirationDate", row.ExpirationDate },
                { "Active", row.Active },
                { "IsOverride", row.IsOverride }
            };

            for (var x = 0; x < row.KeyValues.Count(); x++)
            {
                metadata[table.Keys[x].Name] = row.KeyValues[x].LowValue;

                if (!string.IsNullOrEmpty(row.KeyValues[x].HighValue))
                    metadata[table.Keys[x].Name] += "::" + row.KeyValues[x].HighValue;
            }

            for (var x = 0; x < row.ColumnValues.Count; x++)
            {
                metadata[table.Columns[x].Name] = row.ColumnValues[table.Columns[x].Name];
            }

            return new LookupTableRowModel
            {
                RowId = Convert.ToInt64(row.RowId),
                ChangeId = Guid.Parse(row.ChangeId),
                Sequence = row.Sequence,
                IsOverride = row.IsOverride,
                EffectiveDate = row.EffectiveDate,
                Active = row.Active,
                Values = metadata.Select(v => v.Value.ToString()).ToArray(),
                ItemValues = metadata,
                TableId = table.Id,
                TableName = table.Name
            };
        }

        //public LookupTable ToLookupTable(DataTable table)
        //{
        //    if (!table.Rows.Cast<DataRow>().Any())
        //        return new LookupTable();

        //    var lookupTable = new LookupTable();

        //    var xml = XElement.Parse(table.Rows.Cast<DataRow>().First()[0].ToString());
        //    var tableXml = xml.Element("table");

        //    if (tableXml != null)
        //    {
        //        lookupTable.Name = tableXml.Element("name").Value;
        //        lookupTable.Properties.Schema = tableXml.Element("schema").Value;
        //        lookupTable.Properties.EffectiveDate = tableXml.Element("effectiveDate").Value;

        //        foreach (var row in tableXml.Element("rows").Elements("row"))
        //        {
        //            var columns = row.Elements().Where(x => !MetadataColumnNames.Contains(x.Name.ToString()));

        //        }
        //    }

        //    return lookupTable;
        //}

        public LookupTable ToLookupTable(IDictionary<string, object> parameters, DataTable table)
        {
            var dataTableColumns = table.Columns.Cast<DataColumn>().ToList();

            var metadataXml = XElement.Parse(Convert.ToString(parameters["Metadata"]));
            var factorKeyColumns = new List<FactorTableColumnDefinition>();
            var factorNonKeyColumns = new List<FactorTableColumnDefinition>();
            var metadataColumns = new List<FactorTableColumnDefinition>();

            var columnXml = metadataXml.Elements("ColumnMetadata");

            foreach (var column in columnXml.Elements("Column"))
            {
                //get the base properties
                var columnName = column.Element("Name");

                var columnDefinition = new FactorTableColumnDefinition
                {
                    Name = columnName != null ? columnName.Value : string.Empty
                };

                columnDefinition.DataType =
                    dataTableColumns.Single(c => c.ColumnName == columnDefinition.Name).DataType;

                //get the extended properties
                if (column.Elements("Properties").Descendants().Any())
                {
                    foreach (var property in column.Elements("Properties").Elements("Property"))
                    {
                        var propertyName = property.Element("Name");
                        if (propertyName != null)
                        {
                            var propertyValue = property.Element("Value");

                            if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.Value))
                            {
                                if (propertyName.Value == "IsKey")
                                    columnDefinition.IsKey = propertyValue.Value == "1";
                                else if (propertyName.Value == "LookupType")
                                    columnDefinition.LookupType =
                                        (LookupType)Enum.Parse(typeof(LookupType), propertyValue.Value);
                            }
                        }
                    }
                }

                if (MetadataColumnNames.Contains(columnDefinition.Name))
                    metadataColumns.Add(columnDefinition);
                else if (columnDefinition.IsKey)
                    factorKeyColumns.Add(columnDefinition);
                else
                    factorNonKeyColumns.Add(columnDefinition);
            }

            var properties = new ExpandoObject() as IDictionary<string, object>;
            foreach (var property in metadataXml.Elements().Where(e => e.Name != "ColumnMetadata"))
            {
                properties.Add(property.Name.ToString(), property.Value);
            }

            var lookupTable = new LookupTable(properties["Table"].ToString(),
                metadataColumns.ToArray(),
                factorKeyColumns.ToArray(),
                factorNonKeyColumns.ToArray())
            {
                Id = parameters["ChangeId"].ToString(),
                Properties = properties
            };

            foreach (var row in table.Rows.Cast<DataRow>())
            {
                var lookupTableRow = new LookupTableRow
                {
                    Active = Convert.ToBoolean(row["Active"]),
                    EffectiveDate = Convert.ToDateTime(row["EffectiveDate"]),
                    ExpirationDate = Convert.ToDateTime(row["ExpirationDate"]),
                    RowId = row["Id"].ToString(),
                    Sequence = Convert.ToInt32(row["Sequence"]),
                    ChangeId = row["ChangeId"].ToString(),
                    IsOverride = Convert.ToBoolean(row["IsOverride"].ToString())
                };

                foreach (var key in lookupTable.Keys)
                {
                    lookupTableRow.KeyValues.Add(new LookupTableKey(key.Name, row[key.Name].ToString(), null));
                }

                foreach (var col in lookupTable.Columns)
                {
                    lookupTableRow.ColumnValues.Add(col.Name, row[col.Name]);
                }

                lookupTable.Rows.Add(lookupTableRow);
            }

            return lookupTable;
        }
    }
}
