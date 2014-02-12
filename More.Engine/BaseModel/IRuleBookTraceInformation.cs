using System.Collections.Generic;
using More.Engine.Model;

namespace More.Engine.BaseModel
{
    /// <summary>
    /// A tree model interface for storing trace information
    /// </summary>
    public interface IRuleBookTraceInformation
    {
        #region Data Members (1)

        List<RuleBookTraceInformation> TraceInformation { get; set; }

        #endregion Data Members
    }
}