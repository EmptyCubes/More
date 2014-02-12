using System;
using System.Collections.Generic;
using System.Linq;

namespace More.Engine.Model
{
    public class LookupTableRowModel
    {
        public bool Active { get; set; }

        public Guid ChangeId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int ExceptionId { get; set; }

        public long RowId { get; set; }

        public string TableId { get; set; }

        public string TableName { get; set; }

        public string[] Values { get; set; }

        #region RatingEngineExtensions

        public bool IsOverride { get; set; }

        public object[] ItemArray
        {
            get { return ItemValues.Values.ToArray(); }
        }

        public Dictionary<string, object> ItemValues { get; set; }

        public int Sequence { get; set; }

        public object this[string columnName]
        {
            get { return ItemValues[columnName]; }
            set { ItemValues[columnName] = value; }
        }

        #endregion RatingEngineExtensions
    }
}