using System;

namespace More.Engine.Model
{
    public class RuleBookAssemblyModel
    {
        public DateTime EffectiveDate { get; set; }

        public string Id { get; set; }

        public DateTime? LastCompileDate { get; set; }

        public string Name { get; set; }

        public bool Published { get; set; }
    }
}