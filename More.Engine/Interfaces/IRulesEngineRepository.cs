using System;
using System.Collections.Generic;
using System.Reflection;
using More.Engine.Compiling;
using More.Engine.Model;

namespace More.Application.BaseModel
{
    public interface IRulesEngineRepository
    {
        void DeleteRule(int id);

        void DeleteRuleBook(int id);

        IEnumerable<RuleBookModel> GetAllRuleBooks();

        IEnumerable<RuleBookAssemblyModel> GetRatingAssemblies();

        Assembly GetRatingAssembly(string assemblyPath, string assemblyName, DateTime effectiveDate);

        RuleEngineRule GetRule(int ruleId);

        RuleBookModel GetRuleBook(int id, bool includeDerived = false);

        IEnumerable<RuleEngineRule> GetRuleBookRules(int ruleBookId, Predicate<RuleEngineRule> filter = null, bool validate = true);

        IEnumerable<RuleBookModel> GetRuleBooks();

        void PublishRatingAssembly(DateTime date);

        void SaveRule(RuleEngineRule ratingRule, bool forceNew = false);

        void SaveRuleAssembly(string assemblyName, DateTime effectiveDate);

        void SaveRuleBook(RuleBookModel model);
    }
}