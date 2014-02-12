using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using More.Application;
using More.Application.BaseModel;
using More.Services.WebAPI.Models;

namespace More.Services.WebAPI.Controllers
{
    public class LookupTableController : ApiController
    {
        private IRatingEngineTableRepository _ratingEngineTableRepository;
        public IRatingEngineTableRepository RatingEngineTableRepository
        {
            get
            {
                return _ratingEngineTableRepository ??
                       (_ratingEngineTableRepository =
                           MoreApplicationFactory.GetRatingEngineTableRepository(DateTime.Now));
            }
        }

        //GET: lookupTable/id/{id}
        [HttpGet]
        public DomainTableModel GetTableById(string id)
        {
            var table = RatingEngineTableRepository.GetLookupTable(id);

            //TODO: put in adapter
            var domainTable = new DomainTableModel
            {
                Name = table.Name, 
                Group = table.Properties.Schema,
                TableType = table.Properties.TableType,
                EffectiveDate = DateTime.Parse(table.Properties.EffectiveDate),
                Columns = table.KeysAndColumns.Select(c => new DomainTableColumnModel
                {
                    Name = c.Name,
                    MatchType = c.LookupType.ToString(),
                    IsKey = c.IsKey
                }).ToArray(),
                Rows = table.Rows.Select(r => r.ColumnValues.ToDictionary(d => d.Key, d => d.Value.ToString())).ToList()
            };

            return domainTable;
        }

        //GET: lookupTable/id/{id}/{effectiveDate}
        [HttpGet]
        public DomainTableModel GetTableByIdByEffectiveDate(string id, DateTime effectiveDate)
        {
            RatingEngineTableRepository.ChangeEffectiveDate(effectiveDate);
            var table = RatingEngineTableRepository.GetLookupTable(id);

            //TODO: put in adapter
            var domainTable = new DomainTableModel
            {
                Name = table.Name,
                Group = table.Properties.Schema,
                TableType = table.Properties.TableType,
                EffectiveDate = DateTime.Parse(table.Properties.EffectiveDate),
                Columns = table.KeysAndColumns.Select(c => new DomainTableColumnModel
                {
                    Name = c.Name,
                    MatchType = c.LookupType.ToString(),
                    IsKey = c.IsKey
                }).ToArray(),
                Rows = table.Rows.Select(r => r.KeyValues.Select(k => new { Name = k.Name, Value = k.LowValue })
                                                         .Union(r.ColumnValues.Select(c => new { Name = c.Key, Value = c.Value.ToString() }))
                                 .ToDictionary(k => k.Name, k => k.Value)).ToList()
            };

            return domainTable;
        }

        //GET: lookupTable/{name}/{group}
        [HttpGet]
        public DomainTableModel GetTableByNameByGroup(string name, string group)
        {
            var table = RatingEngineTableRepository.GetLookupTable(name, group, null);

            //TODO: put in adapter
            var domainTable = new DomainTableModel
            {
                Name = table.Name,
                Group = table.Properties.Schema,
                TableType = table.Properties.TableType,
                EffectiveDate = DateTime.Parse(table.Properties.EffectiveDate),
                Columns = table.KeysAndColumns.Select(c => new DomainTableColumnModel
                {
                    Name = c.Name,
                    MatchType = c.LookupType.ToString(),
                    IsKey = c.IsKey
                }).ToArray(),
                Rows = table.Rows.Select(r => r.KeyValues.Select(k => new { Name = k.Name, Value = k.LowValue })
                                                         .Union(r.ColumnValues.Select(c => new { Name = c.Key, Value = c.Value.ToString() }))
                                 .ToDictionary(k => k.Name, k => k.Value)).ToList()
            };

            return domainTable;
        }

        //POST: lookupTable/{name}/{group}/{effectiveDate}/{expression}
        [HttpPost]
        public DomainTableModel GetTableByNameByGroupByEffectiveDate(DomainTableRequestModel model)
        {
            var table = RatingEngineTableRepository.GetLookupTable(model.Name, model.Group, model.EffectiveDate);

            //TODO: put in adapter
            var domainTable = new DomainTableModel
            {
                Name = table.Name,
                Group = table.Properties.Schema,
                TableType = table.Properties.TableType,
                EffectiveDate = DateTime.Parse(table.Properties.EffectiveDate),
                Columns = table.KeysAndColumns.Select(c => new DomainTableColumnModel
                {
                    Name = c.Name,
                    MatchType = c.LookupType.ToString(),
                    IsKey = c.IsKey
                }).ToArray(),
                Rows = table.Rows.Select(r => r.KeyValues.Select(k => new { Name = k.Name, Value = k.LowValue })
                                                         .Union(r.ColumnValues.Select(c => new { Name = c.Key, Value = c.Value.ToString() }))
                                 .ToDictionary(k => k.Name, k => k.Value)).ToList()
            };

            if(model.Filter != null)
                domainTable = ApplyFilter(domainTable, model.Filter);

            return domainTable;
        }

        //TODO: add more flexibility
        private static DomainTableModel ApplyFilter(DomainTableModel model, dynamic filter)
        {
            //filter rows
            foreach (var rowFilter in filter.rows)
            {
                var column = (string)rowFilter.col.ToString();
                var operation = (string)rowFilter.op.ToString();
                var val = (string)rowFilter.val.ToString();

                var outVal = 0.0;
                var isNumeric = double.TryParse(val, out outVal);

                if (operation.Equals("eq"))
                    model.Rows = model.Rows.Where(r => r[column] == val).ToList();
                else if (operation.Equals("lt") && isNumeric)
                    model.Rows = model.Rows.Where(r => double.Parse(r[column]) < outVal).ToList();
                else if (operation.Equals("lte") && isNumeric)
                    model.Rows = model.Rows.Where(r => double.Parse(r[column]) < outVal).ToList();
                else if (operation.Equals("gt") && isNumeric)
                    model.Rows = model.Rows.Where(r => double.Parse(r[column]) < outVal).ToList();
                else if (operation.Equals("gte") && isNumeric)
                    model.Rows = model.Rows.Where(r => double.Parse(r[column]) < outVal).ToList();
                else if (operation.Equals("neq"))
                    model.Rows = model.Rows.Where(r => r[column] != val).ToList();
                else if (operation.Equals("in"))
                    model.Rows = model.Rows.Where(r => val.Split('|').ToList().Contains(r[column])).ToList();
            }

            //filter columns
            var columns = new List<string>();
            foreach (var column in filter.columns)
            {
                columns.Add(column.ToString());
            }

            model.Columns = model.Columns.Where(c => columns.Contains(c.Name)).ToArray();

            model.Rows = model.Rows.Select(r => r.Where(i => columns.Contains(i.Key)).ToDictionary(d => d.Key, d => d.Value)).ToList();
            
            return model;
        }
    }
}
