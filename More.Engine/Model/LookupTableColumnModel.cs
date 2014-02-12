namespace More.Engine.Model
{
    public enum LookupType
    {
        ExactMatch,
        LessThanOrEqual,
        GreaterThanOrEqual,
        GreaterThan,
        LessThan,
        Interpolate,
    }

    public class LookupTableColumnModel
    {
        public LookupType LookupType { get; set; }

        public string Name { get; set; }

        public string TableId { get; set; }

        public string TableName { get; set; }
    }
}