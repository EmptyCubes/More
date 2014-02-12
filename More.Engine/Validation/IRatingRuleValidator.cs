using System.Collections.Generic;
using More.Engine.Model;

namespace More.Engine.Validation
{
    public interface IRuleValidator
    {
        IEnumerable<RuleBookError> ValidateRule(RuleEngineRule rule);
    }
}