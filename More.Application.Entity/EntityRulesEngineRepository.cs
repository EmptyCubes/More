using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using More.Application.BaseModel;
using More.Engine.BaseModel;
using More.Engine.Compiling;
using More.Engine.Model;
using More.Engine.Validation;

namespace More.Application.Entity
{


    public class EntityRulesEngineRepository : IRulesEngineRepository
    {
        public EntityRulesEngineRepository()
        {
        }

        #region Fields (1)

        private RuleBookRule[] _rules;

        #endregion Fields

        #region Properties (5)

        public EntityRulesEngineRepository(DateTime effectiveDate)
        {
            EffectiveDate = effectiveDate.Date;
        }

        public EntityRulesEngineRepository(DateTime effectiveDate, MoreEntities db)
        {
            EffectiveDate = effectiveDate.Date;
            Db = db;
        }

        public MoreEntities Db { get; set; }

        public DateTime EffectiveDate { get; set; }

        public RuleBookRule[] Rules
        {
            get { return _rules ?? (Rules = Db.GetRuleBookRules(EffectiveDate).ToArray()); }
            set { _rules = value; }
        }

        public bool SetupMode { get; set; }

        #endregion Properties

        #region Methods (16)

        // Public Methods (12) 

        public void DeleteRuleBook(int id)
        {
            var item = Db.RuleBooks.FirstOrDefault(p => p.Id == id);
            if (item == null) return;
            if (SetupMode)
            {
                Db.DeleteObject(item);
                Db.SaveChanges();
                return;
            }
            else
            {
                var exception = GetRuleBook(id);
                exception.Id = "0";
                exception.Active = false;
                SaveRuleBook(exception);
            }
        }

        public void DeleteRule(int id)
        {
            var rule = Db.RuleBookRules.First(p => p.Id == id);
            if (SetupMode || rule.EffectiveDate == EffectiveDate)
            {
                Db.DeleteObject(rule);
                Db.SaveChanges();
                return;
            }
            else
            {
                var r = GetRule(id);
                r.Id = "0";
                r.Active = false;
                SaveRule(r);
            }
        }

        public RuleBookModel GetRuleBook(int id, bool includeDerived = false)
        {
            var item = Db.RuleBooks.FirstOrDefault(p => p.Id == id);

            if (item == null) return new RuleBookModel();

            return RateBook(item, includeDerived ? GetDerivedRuleBooks(item) : null);
        }

        public RuleBookModel[] GetDerivedRuleBooks(RuleBook rb)
        {
            return Db.RuleBooks.Where(p => p.BaseChangeId == rb.ChangeId).ToArray().Select(p=>RateBook(p,GetDerivedRuleBooks(p))).ToArray();
        }
        public IEnumerable<RuleBookModel> GetRuleBooks()
        {
            return Db.RuleBooks.Where(p=>p.BaseChangeId == null).ToArray().Select(p=>RateBook(p,GetDerivedRuleBooks(p)));
        }

        public IEnumerable<RuleBookModel> GetAllRuleBooks()
        {
            return Db.RuleBooks.ToArray().Select(p => RateBook(p, null));
        }

        public IEnumerable<RuleBookAssemblyModel> GetRatingAssemblies()
        {
            return Db.RuleAssemblies.ToArray().Select(p => new RuleBookAssemblyModel()
                                                                 {
                                                                     Id = p.Id.ToString(),
                                                                     Name = p.AssemblyName,
                                                                     EffectiveDate = p.EffectiveDate,
                                                                     LastCompileDate = p.LastCompileDate,
                                                                     Published = p.Published
                                                                 }).ToArray();
        }

        public Assembly GetRatingAssembly(string assemblyPath, string assemblyName, DateTime effectiveDate)
        {

            var ratingAssembly =
                Db.RuleAssemblies.FirstOrDefault(p => p.AssemblyName == assemblyName && effectiveDate == EffectiveDate);
            if (ratingAssembly != null)
            {
                return RatingEngineCompiler.GetAssemblyFromFile(Path.Combine(assemblyPath, assemblyName + ".dll"));
            }
            return null;
        }

        public IEnumerable<RuleEngineRule> GetRuleBookRules(int ruleBookId, Predicate<RuleEngineRule> filter, bool validate = true)
        {
            var root = new RuleEngineRule();

            var rules = GetBaseRuleBookRules(ruleBookId).ToArray();

            FillRules(filter, validate, root, rules.Where(p => p.ParentChangeId == null), rules);

            return root.Children;

        }

        public IEnumerable<RuleBookRule> GetBaseRuleBookRules(int ruleBookId)
        {
            var poly = GetBaseRuleBooks(ruleBookId);
            var list = new List<RuleBookRule>();
            RuleBook lastRuleBook = null;
            foreach (var rb in poly)
            {
                var rules = Db.GetRuleBookRulesByRuleBook(EffectiveDate, rb.Id);
                //var rules = Db.RuleBookRules.Where(p => 
                //    p.RuleBookId == rb.Id && p.EffectiveDate <= EffectiveDate).Distinct(IEqualityComparer<>).ToArray();
                foreach (var rule in rules)
                {
                    var current = list.FirstOrDefault(p => p.Name == rule.Name && rule.ParentChangeId == p.ParentChangeId);
                    rule.IsChange = rule.EffectiveDate == EffectiveDate && rule.Tag != "1" && rule.RuleBookId == ruleBookId;
                    rule.IsNew = rule.EffectiveDate == EffectiveDate && rule.Tag == "1" && rule.RuleBookId == ruleBookId && !rule.IsOverride;
                    if (current != null)
                    {
                        current.BaseRules.Add(rule.ChangeId);

                        current.IsOverride = current.RuleBookId == ruleBookId;
                        if (current.IsNew && current.IsOverride)
                        {
                            current.IsNew = false;
                        }
                       
                        continue;
                    }
                    
                    list.Add(rule);
                    yield return rule;
                }
                lastRuleBook = rb;
            }
        }
        public IEnumerable<RuleBook> GetBaseRuleBooks(int ruleBookId)
        {
            RuleBook rb = Db.RuleBooks.First(p => p.Id == ruleBookId);
            yield return rb;
            while (rb != null && rb.BaseChangeId != null)
            {
              
                rb = Db.RuleBooks.Where(p => p.ChangeId == rb.BaseChangeId && p.EffectiveDate <= EffectiveDate ).OrderByDescending(p=>p.EffectiveDate).FirstOrDefault();
                yield return rb;
            }
        }

        private void TraverseRules(RuleEngineRule root, Action<RuleEngineRule> action)
        {
            foreach (var child in root.Children)
            {
                action(child);
                TraverseRules(root, action);
            }
        }

        public RuleEngineRule GetRule(int ruleId)
        {
            var rule = Db.RuleBookRules.FirstOrDefault(p => p.Id == ruleId);
            if (rule != null)
            {
                return CreateRatingRule(null, rule);
            }
            return null;
        }

        public void SaveRuleBook(RuleBookModel model)
        {
            var id = Convert.ToInt32(model.Id);
            var ruleBook = Db.RuleBooks.FirstOrDefault(p => p.Id == id);
            if (ruleBook == null)
            {
                ruleBook = new RuleBook()
                                      {
                                          EffectiveDate = EffectiveDate,
                                          Active = true,
                                          ChangeId = Guid.NewGuid(),
                                      };

                Db.RuleBooks.AddObject(ruleBook);
            }
            ruleBook.BaseChangeId = model.BaseChangeId;
            ruleBook.Name = model.Name;
            ruleBook.Active = model.Active;
            Db.SaveChanges();
        }

        public void SaveRuleAssembly(string assemblyName, DateTime effectiveDate)
        {
            var ratingAssembly =
                Db.RuleAssemblies.FirstOrDefault(p =>
                                                   p.AssemblyName == assemblyName && p.EffectiveDate == effectiveDate
                    );
            if (ratingAssembly == null)
                Db.RuleAssemblies.AddObject(ratingAssembly = new RuleAssembly()
                                                                   {
                                                                       AssemblyName = assemblyName,
                                                                       EffectiveDate = effectiveDate,
                                                                       Published = false
                                                                   });

            ratingAssembly.LastCompileDate = DateTime.Now;
            Db.SaveChanges();

        }

        public void SaveRule(RuleEngineRule ratingRule, bool forceNew = false)
        {
            
            
            var id = Convert.ToInt32(ratingRule.Id);
            var rule = Db.RuleBookRules.FirstOrDefault(p => p.Id == id && p.RuleBookId == ratingRule.RuleBookId);

            if (rule == null || rule.EffectiveDate < EffectiveDate || forceNew)
            {
                rule = new RuleBookRule
                           {
                               ChangeId = new Guid(ratingRule.ChangeId),
                               EffectiveDate = EffectiveDate
                           };

                Db.RuleBookRules.AddObject(rule);
            }

            rule.Context = ratingRule.LoopContext;
            rule.Name = ratingRule.Name;
            rule.RuleBookId = ratingRule.RuleBookId;
            rule.ParentChangeId = ratingRule.ParentChangeId;
            rule.RuleExpression = ratingRule.RuleExpression ?? "true";
            rule.Active = ratingRule.Active;
            Db.SaveChanges();
        }

        // Private Methods (4) 
        private RuleEngineRule CreateRatingRule(IRuleContainer container, RuleBookRule rule)
        {
            return new RuleEngineRule()
            {
                Id = rule.Id.ToString(),
                RuleBookId = rule.RuleBookId,
                ChangeId = rule.ChangeId.ToString(),
                Name = rule.Name,
                Parent = rule.ParentChangeId == null ? null : container as RuleEngineRule,
                EffectiveDate = rule.EffectiveDate,
                ParentChangeId = rule.ParentChangeId,
                RuleExpression = rule.RuleExpression,
                LoopContext = rule.Context,
                Tag = rule.Tag,
                Active = rule.Active
            };
        }

        private void FillRules(Predicate<RuleEngineRule> filter, bool validate, IRuleContainer container, IEnumerable<RuleBookRule> rules, RuleBookRule[] allRules)
        {


            foreach (var rule in rules)
            {
                var treeNode = CreateRatingRule(container, rule);

                if (container is RuleEngineRule)
                    treeNode.Parent = (RuleEngineRule)container;
                treeNode.IsOverride = rule.IsOverride;
                treeNode.HasErrors = treeNode.Expression.HasErrors();
                treeNode.IsNew = rule.IsNew;
                treeNode.IsChange = rule.IsChange;
                // Check the rule for any errors
                if (validate && Validator != null)
                {
                    treeNode.ValidationErrors = Validator.ValidateRule(treeNode).ToArray();
                    if (!treeNode.HasErrors)
                        treeNode.HasErrors = treeNode.ValidationErrors.Length > 0;
                }


                RuleBookRule rule1 = rule;
                FillRules(filter, validate, treeNode, allRules.Where(p => p.ParentChangeId == rule1.ChangeId || rule1.BaseRules.Contains(p.ParentChangeId)).ToArray(), allRules);
                if (filter != null && filter(treeNode))
                {
                    container.Children.Add(treeNode);
                }
                else if (filter == null)
                {
                    container.Children.Add(treeNode);
                }
            }


            var children = container.Children.ToArray();
            // Find the exceptions
            var overrides = children.Where(p => p.RuleBookId > 0).ToArray().Select(p => p.Name);
            // Find the common ones to remove
            var remove = children.Where(p => overrides.Contains(p.Name) && p.RuleBookId == 0);
            // Remove any children with the same name
            foreach (var child in remove)
                container.Children.Remove(child);
        }

        public static IRuleValidator Validator { get; set; }

        private static RuleBookModel RateBook(RuleBook p,RuleBookModel[] derived)
        {
            return new RuleBookModel(p.Id.ToString(), p.Name, p.ChangeId.ToString(), p.EffectiveDate)
            {
                Active = p.Active, 
                BaseChangeId = p.BaseChangeId,
                DerivedRuleBooks = derived
            };
        }
        
        #endregion Methods

        public void PublishRatingAssembly(DateTime date)
        {
            var assembly = Db.RuleAssemblies.FirstOrDefault(p => p.EffectiveDate == date);
            if (assembly == null) return;
            assembly.Published = true;
            Db.SaveChanges();
        }
    }

    public partial class RuleBookRule
    {
        private List<Guid?> _baseRules;
        public bool IsOverride { get; set; }

        public List<Guid?> BaseRules
        {
            get { return _baseRules ?? (_baseRules = new List<Guid?>()); }
            set { _baseRules = value; }
        }

        public bool IsNew { get; set; }
        public bool IsChange { get; set; }
    }
}
