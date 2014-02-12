using System.Linq;
using More.Engine.Compiling;
using More.Engine.Model;

namespace More.Engine.CodeGen.Templates.Model
{
    public class RuleBookTemplateModel
    {
        private string[] _lookups;

        private RuleEngineRule[] _rules;

        public string BaseClassName { get; set; }

        public string ClassName { get { return Name + "RuleBook"; } }

        public string[] Lookups { get { return _lookups ?? (_lookups = Rules.SelectMany(p => p.AllLookups).ToArray()); } }

        public string Name { get; set; }

        public RuleEngineRule[] Rules
        {
            get { return _rules; }
            set { _rules = RuleHelper.SortRules(value).ToArray(); }
        }

        public LookupTable[] Tables { get; set; }

        public RuleBookTemplateModel(string name, RuleEngineRule[] rules, LookupTable[] tables)
            : this(name, "CompiledRuleBookBase", rules, tables)
        {
        }

        public RuleBookTemplateModel(string name, string baseClassName, RuleEngineRule[] rules, LookupTable[] tables)
        {
            Name = name;
            BaseClassName = baseClassName;
            _rules = rules;
            Tables = tables;
        }
    }
}