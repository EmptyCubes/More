using System.Collections.Generic;
using More.Engine.Model;

namespace More.Application.BaseModel
{
    public interface IRulesEngineImporter
    {
        IEnumerable<LookupTable> GetImportLists();

        IEnumerable<LookupTable> GetImportTables();
    }
}