using System;
using System.Linq;
using System.Web.Mvc;
using More.Application.BaseModel;
using More.Engine.Model;

namespace More.Web.Designer.Controllers
{
    public class LookupTablesController : BaseController
    {
        //
        // GET: /LookupTables/

        [HttpPost]
        public ActionResult AddLookupTable(string name, string keys, string columns, string data = null)
        {
            //var columnsLine = lines.First();
            var keyColumns = keys.Split("|".ToCharArray());
            var valueColumns = columns.Split("|".ToCharArray());
            var allColumns = keyColumns.Concat(valueColumns);

            // Create the table
            //var ftId = Guid.NewGuid();
            var rt = new LookupTable()
            {
                Name = name,
                Keys = (from key in keyColumns
                        select new FactorTableColumnDefinition(key, true)).ToArray(),
                Columns = (from valueColumn in valueColumns
                           select new FactorTableColumnDefinition(valueColumn, false)).ToArray(),
            };

            TableStrategy.ImportTables(new LookupTable[] { rt }, false);

            // Import and data
            if (data != null)
            {
                var lines = data.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var dataLines = lines.Skip(1);
                foreach (var dataLine in dataLines)
                    TableStrategy.SaveLookupTableRow(new LookupTableRowModel()
                                                         {
                                                             ChangeId = Guid.NewGuid(),
                                                             TableName = name,
                                                             EffectiveDate = MissionControlDate,
                                                             //RowId = Guid.NewGuid().ToString(),
                                                             Active = true,
                                                             Values =
                                                                 dataLine.Split("|".ToCharArray()).Select(p => p.Trim())
                                                                 .ToArray()
                                                         });
            }
            return View("Index", TableStrategy.GetRatingTables());
        }

        public ActionResult AddLookupTableForm()
        {
            return View();
        }

        public ActionResult DeleteLookupTableKey(int id)
        {
            TableStrategy.DeleteLookupTableKey(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult DeleteLookupTableColumn(int id)
        //{
        //    TableStrategy.DeleteRateTableColumn(id);
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult DeleteLookupTableRow(string lookupTableName, string lookupTableId, string id)
        {
            TableStrategy.DeleteLookupTableRow(lookupTableName, lookupTableId, id);
            return View("LookupTable", TableStrategy.GetLookupTable(lookupTableId));
        }

        public ActionResult ExportTables()
        {
            //var builder = new StringBuilder();
            //var tables = (TableStrategy as IRatingDecorator).DecorateTables(false);

            //foreach (var ratingtable in tables)
            //{
            //    var allColumns = ratingtable.Keys.Concat(ratingtable.Columns).ToArray();
            //    var tableBuilder = new TableBuilder { Fields = allColumns.Select(p => FixColumnName(p.Name)).ToList(), ClassName = ratingtable.Name };

            //    builder.AppendLine(tableBuilder.CreateTableScript());
            //    builder.AppendLine("GO");
            //    foreach (var column in allColumns)
            //    {
            //        builder.AppendFormat(
            //            "EXEC sys.sp_addextendedproperty @name=N'IsKey', @value=N'{0}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{1}', @level2type=N'COLUMN',@level2name=N'{2}'", column.IsKey ? "1" : "0", ratingtable.Name, FixColumnName(column.Name)).AppendLine();
            //        builder.AppendLine("GO");
            //    }

            //    foreach (var row in ratingtable.Rows)
            //    {
            //        var guid = Guid.NewGuid();
            //        var values = row.KeyValues.Select(
            //            p => p.LowValue + (string.IsNullOrEmpty(p.HighValue) ? string.Empty : "::" + p.HighValue)).
            //            Concat(row.ColumnValues.Select(p => p.Value)).ToArray();

            //        builder.AppendFormat(
            //            "INSERT INTO {0} ([{1}],[ChangeId], [EffectiveDate], [Active]) VALUES( '{2}','{3}','{4}', 1 )",
            //            ratingtable.Name,
            //            string.Join("],[", allColumns.Select(p => FixColumnName(p.Name))),
            //            string.Join("','", values),
            //            guid.ToString(),
            //            new DateTime(2012, 02, 22)

            //            ).AppendLine().AppendLine("GO");
            //    }
            //}

            //return Content(builder.ToString());
            return View();
        }

        public ActionResult Index(string id = null)
        {
            if (id != null)
            {
                return View("LookupTable", TableStrategy.GetLookupTable(id));
            }
            return View(TableStrategy.GetRatingTables());
        }

        public ActionResult LookupTableKey(string lookupTableName, string lookupTableId, string keyName)
        {
            return View(TableStrategy.GetLookupTableKey(lookupTableName, lookupTableId, keyName));
        }

        public ActionResult LookupTableRow(string lookupTableName, string lookupTableId, string id = null)
        {
            return View(TableStrategy.GetLookupTableRow(lookupTableName, lookupTableId, id));
        }

        public ActionResult RateTableProperties(string id)
        {
            return View(TableStrategy.GetRatingTableModel(id) ?? new LookupTableModel()
            {
                ChangeId = Guid.NewGuid().ToString(),
            });
        }

        public ActionResult SaveLookupTableColumn(LookupTableColumnModel model)
        {
            return View(TableStrategy.GetLookupTable(model.TableId));
        }

        //public ActionResult SaveRateTable(LookupTableModel model)
        //{
        //    TableStrategy.SaveRatingTableModel(model);
        //    return View("RatingTables", TableStrategy.GetRatingTables());
        //}
        public ActionResult SaveLookupTableKey(LookupTableKeyModel model)
        {
            TableStrategy.SaveLookupTableKey(model);
            return View(TableStrategy.GetLookupTable(model.TableId));
        }

        public ActionResult SaveLookupTableRow(LookupTableRowModel model)
        {
            TableStrategy.SaveLookupTableRow(model);
            return View("LookupTable", TableStrategy.GetLookupTable(model.TableId));
        }

        public ActionResult SaveRateTableRow(LookupTableRowModel model)
        {
            TableStrategy.SaveLookupTableRow(model);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}