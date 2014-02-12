using System.Collections.Generic;
using System.Linq;
using More.Engine.Model;

namespace More.Engine.Validation
{
    public class RatingRuleValidator : IRuleValidator
    {
        public string[] LookupTables { get; set; }

        public string[] PredefinedFunctions { get; set; }

        public IEnumerable<RuleBookError> ValidateRule(RuleEngineRule rule)
        {
            foreach (var lookup in rule.Lookups)
            {
                if (!LookupTables.Contains(lookup) && !PredefinedFunctions.Contains(lookup))
                {
                    yield return new RuleBookError("Validator", string.Format("Lookup/Method {0} not found.", lookup));
                }
            }
        }
    }
}