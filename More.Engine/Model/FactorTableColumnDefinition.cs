using System;
using System.Collections.Generic;
using More.Application.BaseModel;

namespace More.Engine.Model
{
    /// <summary>
    /// Definition model class of a factor table column/key definition
    /// </summary>
    public class FactorTableColumnDefinition
    {
        private Dictionary<string, object> _properties;

        public Guid ChangeId { get; set; }

        public Guid FactorTableChangeId { get; set; }

        public int ID { get; set; }

        public bool IsKey { get; set; }

        public LookupType LookupType { get; set; }

        public string Name { get; set; }

        public Dictionary<string, object> Properties
        {
            get { return _properties ?? (_properties = new Dictionary<string, object>()); }
            set { _properties = value; }
        }

        public FactorTableColumnDefinition()
        {
        }

        public FactorTableColumnDefinition(string name, bool isKey)
        {
            Name = name;
            IsKey = isKey;
        }

        public FactorTableColumnDefinition(int id, Guid changeId, Guid factorTableChangeId, string name, bool isKey)
        {
            ID = id;
            ChangeId = changeId;
            FactorTableChangeId = factorTableChangeId;
            Name = name;
            IsKey = isKey;
        }

        public FactorTableColumnDefinition(int id, Guid changeId, Guid factorTableChangeId, string name, bool isKey,
            Type dataType)
            : this(id, changeId, factorTableChangeId, name, isKey)
        {
            DataType = dataType;
        }

        #region RatingEngineExtensions

        public Type DataType { get; set; }

        #endregion RatingEngineExtensions
    }
}