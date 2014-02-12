using System.Collections.Generic;

namespace More.Engine.Model
{
    public class ImporterModel
    {
        public string ImportFile { get; set; }

        public IEnumerable<ImporterTableModel> Lists { get; set; }

        public IEnumerable<ImporterTableModel> Tables { get; set; }
    }
}