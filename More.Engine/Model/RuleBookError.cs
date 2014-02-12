using System;

namespace More.Engine.Model
{
    [Serializable]
    public class RuleBookError
    {
        public Exception Exception { get; set; }

        //public RuleEngineRule Rule { get; set; }
        public string Message { get; set; }

        public string RuleDescription { get; set; }

        public string RuleId { get; set; }

        public string Type { get; set; }

        public RuleBookError()
        {
        }

        public RuleBookError(string type, string message, string ruleId, string ruleDescription)
            : this(type, message, null, ruleId, ruleDescription)
        {
        }

        public RuleBookError(string type, string message, Exception exception, string ruleId, string ruleDescription)
        {
            Type = type;
            Message = message;
            Exception = exception;
            RuleId = ruleId;
            RuleDescription = ruleDescription;
        }

        public RuleBookError(string type, string message)
        {
            Message = message;
            //Rule = rule;
            this.Type = type;
        }
    }
}