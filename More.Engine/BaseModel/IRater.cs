using System.Collections.Generic;
using More.Engine.Model;

namespace More.Engine.BaseModel
{
    /// <summary>
    /// A basic interface for a rating engine implementation
    /// </summary>
    public interface IRuleBook
    {
        #region Data Members (1)

        bool IncludeTrace { get; set; }

        #endregion Data Members

        #region Operations (2)

        RuleBookResult Execute(Dictionary<string, object> inputs);

        RuleBookResult Execute(Dictionary<string, object> inputs, int numberTimes);

        #endregion Operations
    }
}