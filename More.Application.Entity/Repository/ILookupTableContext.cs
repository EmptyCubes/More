namespace More.Application.Entity.Repository
{
    public interface ILookupTableContext
    {
        ITableContextCommand GetCommand(LookupTableCommand command);
    }
}
