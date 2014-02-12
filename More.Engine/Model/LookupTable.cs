using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using More.Application.BaseModel;

namespace More.Engine.Model
{
    /// <summary>
    /// This is a model class for a rating table.  It contains the column and key definitions
    /// </summary>
    public class LookupTable
    {
        #region Fields (1)

        private Dictionary<string, object> _properties;
        private List<LookupTableRow> _rows;

        #endregion Fields

        #region Constructors (3)

        public bool Active
        {
            get
            {
                return Properties.Active == "1";
            }
        }

        public FactorTableColumnDefinition[] AllColumns
        {
            get //TODO: derp?
            {
                var columns = new List<FactorTableColumnDefinition>();

                if (MetadataColumns != null)
                    columns.AddRange(MetadataColumns);

                if (Keys != null)
                    columns.AddRange(Keys);

                if (Columns != null)
                    columns.AddRange(Columns);

                return columns.ToArray();
            }
        }

        /// <summary>
        /// The Column/Value Definitions
        /// </summary>
        public FactorTableColumnDefinition[] Columns { get; set; }

        public string Description
        {
            get
            {
                return string.Format("{0} ( columnName, {1} )", Name, string.Join(", ", Keys.Select(p => p.Name)));
            }
        }

        public string Id { get; set; }

        public bool IsList { get; set; }

        /// <summary>
        /// The keys definition
        /// </summary>
        public FactorTableColumnDefinition[] Keys { get; set; }

        public FactorTableColumnDefinition[] KeysAndColumns
        {
            get { return Keys.Concat(Columns).ToArray(); }
        }

        public FactorTableColumnDefinition[] MetadataColumns { get; set; }

        /// <summary>
        /// The name of the tables
        /// </summary>
        public string Name { get; set; }

        public dynamic Properties { get; set; }

        /// <summary>
        /// The actual column & key values the makes a row of data
        /// </summary>
        public List<LookupTableRow> Rows
        {
            get { return _rows ?? (_rows = new List<LookupTableRow>()); }
            set { _rows = value; }
        }

        public Dictionary<string, object> TableProperties
        {
            get { return _properties ?? (_properties = new Dictionary<string, object>()); }
            set { _properties = value; }
        }

        public LookupTable(string name, FactorTableColumnDefinition[] metadata, FactorTableColumnDefinition[] keys,
            FactorTableColumnDefinition[] columns)
            : this(name, keys, columns)
        {
            MetadataColumns = metadata;
        }

        public LookupTable(string name, FactorTableColumnDefinition[] keys, FactorTableColumnDefinition[] columns)
            : this()
        {
            Name = name;
            Keys = keys;
            Columns = columns;
        }

        public LookupTable(string name)
            : this()
        {
            Name = name;
        }

        public LookupTable()
        {
            Properties = new ExpandoObject();
        }

        #endregion Constructors

        #region Properties (4)
        #endregion Properties

        public bool ExactMatch(object a, object b)
        {
            if (a is string && b is string)
            {
                return ((string)a).ToUpper().Equals(((string)b).ToUpper());
            }
            else if (a is string)
            {
                return ((string)a).ToUpper().Equals(b.ToString().ToUpper());
            }
            else if (b is string)
            {
                return ((string)b).ToUpper().Equals(a.ToString().ToUpper());
            }
            else if (a is double || b is double)
            {
                return Convert.ToDouble(a) == Convert.ToDouble(b);
            }
            return a.Equals(b);
        }

        public Func<object, object, bool> GetMatcher(LookupType lookupType)
        {
            switch (lookupType)
            {
                case LookupType.ExactMatch:
                    return ExactMatch;

                case LookupType.GreaterThan:
                    return GreaterThan;

                case LookupType.GreaterThanOrEqual:
                    return GreaterThanOrEqual;

                case LookupType.LessThan:
                    return LessThanOrEqual;

                case LookupType.LessThanOrEqual:
                    return LessThanOrEqual;

                default:
                    return ExactMatch;
            }
        }

        public double GetNumber(object a)
        {
            if (a is int)
            {
                return (int)a;
            }
            return Convert.ToDouble(a);
        }

        public bool GreaterThan(object left, object right)
        {
            return GetNumber(left) > GetNumber(right);
        }

        public bool GreaterThanOrEqual(object left, object right)
        {
            return GetNumber(left) >= GetNumber(right);
        }

        public bool LessThan(object left, object right)
        {
            return GetNumber(left) < GetNumber(right);
        }

        public bool LessThanOrEqual(object left, object right)
        {
            return GetNumber(left) <= GetNumber(right);
        }

        public object Lookup(string column, params object[] keys)
        {
            // Sort the rows in the correct order based on the lookuptype
            var sortedRows = RowsSorted();
            var row = sortedRows.FirstOrDefault(p => p.IsMatch(this, keys));
            if (row == null) return null;
            return row.ColumnValues[column];
        }

        public IEnumerable<LookupTableRow> RowsSorted()
        {
            var sortKey = Keys.FirstOrDefault(
                    p => p.LookupType != LookupType.ExactMatch && p.LookupType != LookupType.Interpolate);

            if (sortKey == null) return Rows;

            var sortColumnIndex = Array.IndexOf(Keys.ToArray(), sortKey);

            switch (sortKey.LookupType)
            {
                case LookupType.GreaterThanOrEqual:
                case LookupType.GreaterThan:
                    return Rows.OrderByDescending(p => Decimal.Parse((string)p.KeyValues[sortColumnIndex].LowValue));

                case LookupType.LessThan:
                case LookupType.LessThanOrEqual:
                    return Rows.OrderBy(p => Decimal.Parse(p.KeyValues[sortColumnIndex].LowValue));
            }
            return Rows;
        }

        public override string ToString()
        {
            return Description;
        }

        #region RatingEngineExtensions
        #endregion
    }
}