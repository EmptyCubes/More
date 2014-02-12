using System.Collections.Generic;

namespace More.Engine.Model
{
    /// <summary>
    /// A rating rule list that keeps track of who owns this list.
    /// This is useful for making sure the 'Parent' property is set
    /// to the correct value when adding a child to this list
    /// </summary>
    public class RulesCollection : List<RuleEngineRule>
    {
        /// <summary>
        /// The Owner of this collection
        /// </summary>
        public RuleEngineRule Owner { get; set; }

        public RulesCollection(IEnumerable<RuleEngineRule> collection, RuleEngineRule owner)
            : base(collection)
        {
            Owner = owner;
        }

        public RulesCollection(RuleEngineRule owner = null)
        {
            Owner = owner;
        }

        public new void Add(RuleEngineRule item)
        {
            item.Parent = Owner;
            base.Add(item);
        }
    }
}