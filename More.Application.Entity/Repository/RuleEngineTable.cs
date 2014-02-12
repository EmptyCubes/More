using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace More.Application.Entity.Repository
{
    public class RuleEngineTable
    {
        public RuleEngineTable()
        {
            Rows = new List<RuleEngineTableRow>();
        }

        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }

        public RuleEngineTableMetadata Metadata { get; set; }
        public List<RuleEngineTableRow> Rows { get; set; }
    }

    public class RuleEngineTableMetadata
    {
        public RuleEngineTableMetadata()
        {
            Columns = new List<RuleEngineColumnMetadata>();
        }

        public List<RuleEngineColumnMetadata> Columns { get; set; }
    }

    public class RuleEngineColumnMetadata
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string DataType { get; set; }

        [BsonIgnoreIfNull]
        public string LookupType { get; set; }
    }

    public class RuleEngineTableRow
    {
        public RuleEngineTableRow()
        {
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            Id = ObjectId.GenerateNewId().ToString();
            Values = new Dictionary<string, object>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Dictionary<string, object> Values { get; set; }
    }
}