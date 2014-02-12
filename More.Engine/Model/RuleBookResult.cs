using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using More.Engine.BaseModel;

namespace More.Engine.Model
{
    [Serializable]
    public class InputRequirement
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// The final result of a rating engine invokation.
    /// It implements IRuleBookTraceInformation because it
    /// has a list of Trace Information
    /// </summary>
    [Serializable]
    public class RuleBookResult : IRuleBookTraceInformation
    {
        #region Fields (3)

        private List<RuleBookError> _errors;

        [IgnoreDataMember]
        private Dictionary<string, object> _results;

        private List<RuleBookTraceInformation> _traceInformation;

        #endregion Fields

        #region Constructors (3)

        [DataMember]
        public List<RuleBookError> Errors
        {
            get { return _errors ?? (_errors = new List<RuleBookError>()); }
            set { _errors = value; }
        }

        [IgnoreDataMember]
        public Dictionary<string, object> Results
        {
            get { return _results ?? (_results = new Dictionary<string, object>()); }
            set { _results = value; }
        }

        public List<RuleBookTraceInformation> TraceInformation
        {
            get { return _traceInformation ?? (_traceInformation = new List<RuleBookTraceInformation>()); }
            set { _traceInformation = value; }
        }

        public RuleBookResult(Dictionary<string, object> results, List<RuleBookError> errors)
        {
            Results = results;
            Errors = errors;
        }

        public RuleBookResult(Dictionary<string, object> results)
        {
            Results = results;
        }

        public RuleBookResult()
        {
        }

        #endregion Constructors

        #region Properties (4)

        public object Find(string name)
        {
            return Find(TraceInformation, name);
        }

        public object Find(IEnumerable<RuleBookTraceInformation> info, string name)
        {
            foreach (var child in info)
            {
                if (child.Description.ToUpper() == name.ToUpper())
                {
                    return child.Value;
                }
                var result = Find(child.TraceInformation, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public Dictionary<string, object> Path(params string[] path)
        {
            var current = Results;
            foreach (var p in path)
            {
                current = current[p] as Dictionary<string, object>;
            }
            return current;
        }

        #endregion Properties
    }
}