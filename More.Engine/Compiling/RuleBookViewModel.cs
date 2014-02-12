using System;

namespace More.Engine.Compiling
{
    /// <summary>
    /// A viewModel class for rating engine 'Exceptions'. Very similar to a rating-rule.
    /// But, does not have a tree-like structure
    /// </summary>
    public class RuleBookModel
    {
        private string[] _inputs;

        public bool Active { get; set; }

        public Guid? BaseChangeId { get; set; }

        public string ChangeId { get; set; }

        public RuleBookModel[] DerivedRuleBooks { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public RuleBookModel()
        {
        }

        public RuleBookModel(string id, string name, string changeId, DateTime effectiveDate)
        {
            Id = id;
            Name = name;
            ChangeId = changeId;
            EffectiveDate = effectiveDate;
        }
    }
}