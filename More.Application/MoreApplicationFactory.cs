using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using More.Application.BaseModel;
using More.Application.Entity;
using More.Application.Entity.Repository;
using More.Engine.CodeGen.Templates.Model;
using More.Engine.Compiling;
using More.Engine.Model;
using More.Engine.Validation;

namespace More.Application
{
    public static class MoreApplicationFactory
    {
        public static DateTime GetAssemblyEffectiveDate(DateTime effectiveDate)
        {
            var assemblyEffectiveDate = GetEngineRepository(effectiveDate).GetRatingAssemblies().Where(p => p.EffectiveDate <= effectiveDate).
                OrderByDescending(p => p.EffectiveDate).FirstOrDefault();
            if (assemblyEffectiveDate != null)
            {
                return assemblyEffectiveDate.EffectiveDate;
            }
            throw new Exception("An assembly for the effective date " + effectiveDate.ToString() + "Couldn't  be found. Make sure it has been compiled and published.");
        }

        public static string GetAssemblyName(DateTime effectiveDate)
        {
            return string.Format("Rules{0}{1}{2}", effectiveDate.Month, effectiveDate.Day,
                                 effectiveDate.Year);
        }

        public static RatingEngineCompiler GetCompiler(DateTime effectiveDate)
        {
            var compiler = new RatingEngineCompiler(GetRuleEngineCodeModel(GetAssemblyName(effectiveDate), effectiveDate));

            return compiler;
        }

        /// <summary>
        /// Get the repository that will provide date for engine compiling.
        /// </summary>
        /// <param name="effectiveDate"></param>
        /// <param name="setupMode"></param>
        /// <returns></returns>
        public static IRulesEngineRepository GetEngineRepository(DateTime effectiveDate, bool setupMode = false)
        {
            //EntityRulesEngineRepository.Validator = new RatingRuleValidator()
            //                                            {
            //                                            };
            return new EntityRulesEngineRepository(effectiveDate, new MoreEntities())
            {
                SetupMode = setupMode
            };
        }

        /// <summary>
        /// The lookup repository that will provide lookup tables to the engine.
        /// </summary>
        /// <param name="effectiveDate"></param>
        /// <param name="setupMode"></param>
        /// <returns></returns>
        public static ILookupTablesRepository GetLookupRepository(DateTime effectiveDate, bool setupMode = false)
        {
            return new SqlLookupTableRepository(
                new SqlLookupTableContext(
                    WebConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString),
                    effectiveDate,
                    setupMode);
            //return new FlatTableRatingConfigStrategy(effectiveDate, new MoreLookupTableEntities())
            //{
            //    SetupMode = setupMode
            //};
        }

        public static IRatingEngineTableRepository GetRatingEngineTableRepository(DateTime effectiveDate,
            bool setupMode = false)
        {
            return new SqlLookupTableRepository(new SqlLookupTableContext(
                WebConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString),
                effectiveDate,
                setupMode);
        }

        /// <summary>
        /// A simple method to data from both lookup repository and engine repository to provide a combined model for compiling.
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="ruleBookIds"></param>
        /// <returns></returns>
        public static RuleEngineCodeModel GetRuleEngineCodeModel(string nameSpace, DateTime effectiveDate, string[] ruleBookIds = null)
        {
            var lookupRepository = GetLookupRepository(effectiveDate);
            var engineRepository = GetEngineRepository(effectiveDate);
            var rbs = engineRepository.GetAllRuleBooks().ToArray();
            RuleBookTemplateModel[] ruleBooks = null;

            if (ruleBookIds != null)
            {
                ruleBooks = rbs.Where(p => ruleBookIds.Contains(p.Id)).Select(
                           p => new RuleBookTemplateModel(
                                        p.Name, "CommonRuleBook",
                                        RuleHelper.SortRules(engineRepository.GetRuleBookRules(Convert.ToInt32((string)p.Id), null).ToArray()).ToArray(),
                                        null
                                    )
                        ).ToArray();
            }
            else
            {
                ruleBooks = rbs.Select(
                           p => new RuleBookTemplateModel(
                                        p.Name, "CommonRuleBook",
                                        RuleHelper.SortRules(engineRepository.GetRuleBookRules(Convert.ToInt32((string)p.Id), null).ToArray()).ToArray(),
                                        null
                                    )
                        ).ToArray();
            }

            var model = new RuleEngineCodeModel()
                            {
                                EffectiveDate = effectiveDate,
                                Namespace = nameSpace,
                                RuleBooks = ruleBooks,
                                BaseLookupTables = lookupRepository.GetRatingTables().
                                    Select(p => lookupRepository.GetLookupTable(p.ChangeId)).ToArray()
                            };
            return model;
        }
    }
}