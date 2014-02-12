namespace More.Engine.Model
{
    public class LookupTableKeyModel
    {
        public LookupType LookupType { get; set; }

        public string Name { get; set; }

        public string TableId { get; set; }

        public string TableName { get; set; }
        public string TableSchema { get; set; }
    }
}