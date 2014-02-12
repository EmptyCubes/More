using System;
using System.Collections.Generic;
using More.Engine.BaseModel;

namespace More.Engine.Model
{
    [Serializable]
    public class RuleBookTraceInformation : IRuleBookTraceInformation
    {
        #region Fields (1)

        private List<RuleBookTraceInformation> _children;

        #endregion Fields

        #region Constructors (2)

        public string ContextId { get; set; }

        public string Description { get; set; }

        public string Expression { get; set; }

        public int Id { get; set; }

        public string LoopContext { get; set; }

        public string RuleId { get; set; }

        public List<RuleBookTraceInformation> TraceInformation
        {
            get { return _children ?? (_children = new List<RuleBookTraceInformation>()); }
            set { _children = value; }
        }

        public object Value { get; set; }

        public RuleBookTraceInformation(string ruleId, string expression, string description, object value)
        {
            RuleId = ruleId;
            Expression = expression;
            Description = description;
            Value = value;
        }

        public RuleBookTraceInformation()
        {
        }

        #endregion Constructors

        #region Properties (6)

        public IEnumerable<RuleBookTraceInformation> Find(Predicate<RuleBookTraceInformation> predicate)
        {
            var results = new List<RuleBookTraceInformation>();
            Traverse(p =>
                         {
                             if (predicate(p))
                             {
                                 results.Add(p);
                             }
                         });

            return results;
        }

        public void Traverse(Action<RuleBookTraceInformation> action)
        {
            Traverse(this, action);
        }

        protected void Traverse(RuleBookTraceInformation parent, Action<RuleBookTraceInformation> action)
        {
            foreach (var child in parent.TraceInformation)
            {
                action(child);
                child.Traverse(child, action);
            }
        }

        #endregion Properties
    }
}