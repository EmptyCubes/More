using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using More.Application;
using More.Engine;
using More.Engine.Model;

namespace More.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class MoreService : IMoreService
    {
        public RuleBookTraceInformation[] Execute(DateTime effectiveDate, string ruleBookName, Dictionary<string, object> inputs)
        {
            //MoreApplicationFactory.GetCompiler(effectiveDate);
            var rulesEngine = new RulesEngine();
            var assemblyEffectiveDate = MoreApplicationFactory.GetAssemblyEffectiveDate(effectiveDate);
            var factory = rulesEngine.GetFactory(MoreApplicationFactory.GetAssemblyName(assemblyEffectiveDate),
                MoreApplicationFactory.GetLookupRepository(effectiveDate)
                );
            if (factory == null)
            {
                throw new Exception(
                    "Either the assembly was not found.  Or it was compiled incorrectly. Recompile and try again.");
            }
            var ruleBook = factory.GetRuleBook(ruleBookName);

            if (ruleBook == null)
            {
                throw new Exception("Rule book not found.");
            }
            var result = ruleBook.Execute(inputs);
            if (result.Errors.Any())
            {
                var error = result.Errors.First();
                throw new FaultException(string.Format("{0} - {1}", error.Message, error.RuleDescription));
            }
            return result.TraceInformation.ToArray();
        }

        public InputRequirement[] GetInputRequirements(DateTime effectiveDate, string ruleBookName)
        {
            var rulesEngine = new RulesEngine();
            var assemblyEffectiveDate = MoreApplicationFactory.GetAssemblyEffectiveDate(effectiveDate);
            var factory = rulesEngine.GetFactory(
                MoreApplicationFactory.GetAssemblyName(assemblyEffectiveDate),
                MoreApplicationFactory.GetLookupRepository(effectiveDate));
            if (factory == null)
            {
                throw new Exception(
                    "Either the assembly was not found.  Or it was compiled incorrectly. Recompile and try again.");
            }
            var ruleBook = factory.GetRuleBook(ruleBookName);

            return ruleBook.GetInputRequirements().ToArray();
        }
    }
}