using System;
using System.Data;

namespace More.Application.Entity.Repository
{
    /// <summary>
    /// Command to be executed by a table context instance.
    /// </summary>
    public interface ITableContextCommand : IDisposable
    {
        /// <summary>
        /// Performs the work in executing the tasks for the given table context. Does not return any values.
        /// </summary>
        /// <param name="parameters">A listing of parameters to be used by the command.</param>
        void Execute(params object[] parameters);

        /// <summary>
        /// Performs the work in executing the tasks for the given table context.
        /// </summary>
        /// <param name="parameters">A listing of parameters to be used by the command.</param>
        /// <returns>A DataTable representing the results of the execution.</returns>
        DataTable ExecuteWithData(params object[] parameters);
    }

    /// <summary>
    /// A list of lookup table commands.
    /// </summary>
    public enum LookupTableCommand
    {
        GetTable,
        GetAllTables,
        GetRow,
        UpdateRow,
        InsertRow,
        DeleteRow,
        CreateTable,
        DropTable,
        DeleteTable,
        SaveLookupTableKey,
        GetTableByName
    }
}
