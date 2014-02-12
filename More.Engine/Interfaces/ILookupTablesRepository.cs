using System;
using System.Collections.Generic;
using More.Engine.Model;

namespace More.Application.BaseModel
{
    public interface ILookupTablesRepository
    {
        void DeleteLookupTableKey(int id);

        void DeleteLookupTableRow(string tableName, string tableId, string rowId);

        void DeleteRateTableColumn(int id);

        LookupTable GetLookupTable(string id);

        LookupTableKeyModel GetLookupTableKey(string tableName, string tableId, string tableColumn);

        //LookupTableColumnModel GetRatingTableColumn(int id);
        LookupTableRowModel GetLookupTableRow(string lookupTableName, string lookupTableId, string rowId);

        LookupTableModel GetRatingTableModel(string id);

        IEnumerable<LookupTableModel> GetRatingTables();

        void ImportTables(IEnumerable<LookupTable> importTables, bool drop);

        void SaveLookupTableKey(LookupTableKeyModel model);

        // void SaveRatingTableColumn(LookupTableColumnModel model);
        void SaveLookupTableRow(LookupTableRowModel model);

        void SaveRatingTableModel(LookupTableModel model);
    }

    /// <summary>
    /// Bridge interface to the lookup table repository to add an additional method.
    /// </summary>
    public interface IRatingEngineTableRepository : ILookupTablesRepository
    {
        /// <summary>
        /// Gets the lookup table view models based on the repository's effective date. The predicate
        /// is applied after the tables are retrieved.
        /// </summary>
        /// <param name="predicate">Predicate for filtering the lookup table models.</param>
        /// <returns>An enumeration of lookup table view models.</returns>
        IEnumerable<LookupTableModel> GetRatingTables(Func<LookupTableModel, bool> predicate);
        void ChangeEffectiveDate(DateTime effectiveDate);
        LookupTable GetLookupTable(string name, string schema, DateTime? effectiveDate);
        void DropTables(IEnumerable<LookupTable> dropTables);
    }
}