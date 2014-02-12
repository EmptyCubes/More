namespace More.Application.Entity.Repository
{
    public enum SqlWhereOperator
    {
        GreaterThan,
        LessThan,
        Equal,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        NotEqual
    }

    public class SqlWhereClauseItem
    {
        public string ColumnName { get; set; }
        public object ColumnValue { get; set; }
        public SqlWhereOperator Operator { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(ColumnName))
                return string.Empty;

            var operatorText = string.Empty;
            switch (Operator)
            {
                case SqlWhereOperator.Equal:
                    operatorText = "=";
                    break;
                case SqlWhereOperator.GreaterThan:
                    operatorText = ">";
                    break;
                case SqlWhereOperator.GreaterThanOrEqualTo:
                    operatorText = ">=";
                    break;
                case SqlWhereOperator.LessThan:
                    operatorText = "<";
                    break;
                case SqlWhereOperator.LessThanOrEqualTo:
                    operatorText = "<=";
                    break;
                case SqlWhereOperator.NotEqual:
                    operatorText = "<>";
                    break;
            }

            return FlatTableQueries.IsSqlStringy(ColumnValue)
                ? string.Format("[{0}] {1} {2}", ColumnName, operatorText, ColumnValue)
                : string.Format("[{0}] {1} '{2}'", ColumnName, operatorText, ColumnValue);
        }
    }
}
