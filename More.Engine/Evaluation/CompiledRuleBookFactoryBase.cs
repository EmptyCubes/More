using System;
using System.Collections.Generic;
using More.Application.BaseModel;

namespace More.Engine.Evaluation
{
    public abstract class CompiledRuleBookFactoryBase : MarshalByRefObject
    {
        public virtual DateTime EffectiveDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        public ILookupTablesRepository LookupTablesRepository
        {
            get;
            set;
        }

        public abstract CompiledRuleBookBase GetRuleBook(string name);

        protected object GetInput(string name, Dictionary<string, object> context)
        {
            if (context != null && context.ContainsKey(name))
            {
                return context[name];
            }
            return null;
        }
    }
}