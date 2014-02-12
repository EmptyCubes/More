using System;
using System.Collections.Generic;
using More.Application.BaseModel;

namespace More.Engine.Model
{
    public class LookupTableRow
    {
        private Dictionary<string, object> _columnValues;

        private List<LookupTableKey> _keyValues;

        public bool Active { get; set; }

        public string ChangeId { get; set; }

        public Dictionary<string, object> ColumnValues
        {
            get { return _columnValues ?? (_columnValues = new Dictionary<string, object>()); }
            set { _columnValues = value; }
        }

        public DateTime EffectiveDate { get; set; }

        public int ExceptionId { get; set; }

        public List<LookupTableKey> KeyValues
        {
            get { return _keyValues ?? (_keyValues = new List<LookupTableKey>()); }
            set { _keyValues = value; }
        }

        public string RowId { get; set; }

        #region RatingEngineExtensions

        public DateTime ExpirationDate { get; set; }

        public bool IsOverride { get; set; }

        public int Sequence { get; set; }

        #endregion RatingEngineExtensions

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

        public bool IsMatch(LookupTable table, params object[] keyValues)
        {
            for (int index = 0; index < keyValues.Length; index++)
            {
                // The key value
                var key = keyValues[index];
                // The definition of this key
                var keyDefinition = table.Keys[index];
                // How should we match the keyValues
                var matcher = GetMatcher(keyDefinition.LookupType);
                // Sort the rows in the correct order based on the lookuptype
                var rowKeyValue = KeyValues[index];

                if (!matcher(key, rowKeyValue)) return false;
            }
            return true;
        }

        public bool LessThan(object left, object right)
        {
            return GetNumber(left) < GetNumber(right);
        }

        public bool LessThanOrEqual(object left, object right)
        {
            return GetNumber(left) <= GetNumber(right);
        }
    }
}