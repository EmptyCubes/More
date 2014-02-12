using System;
using More.Engine.Model;

namespace More.Engine.CodeGen.Templates.Model
{
    public class RuleEngineCodeModel
    {
        // var commonTables = Data.DecorateTables(false, lookups.ToArray()).ToArray();
        public LookupTable[] BaseLookupTables { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Namespace { get; set; }

        public RuleBookTemplateModel[] RuleBooks { get; set; }
    }
}