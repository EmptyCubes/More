using System;
using System.Linq;
using System.Web.Mvc;
using More.Engine.Compiling;
using More.Engine.Model;

namespace More.Web.Designer.Controllers
{
    public class RulesController : BaseController
    {
        public ActionResult AssignRuleParent(int ruleId, string parentId)
        {
            var rules = Strategy.GetRuleBookRules(CurrentRuleBookId).ToArray();
            foreach (var ruleEngineRule in rules)
            {
                var parent = ruleEngineRule.Find(parentId);
                if (parent == null) continue;
                var x = parent.Parents.FirstOrDefault(p => p.Id == ruleId.ToString());
                if (x != null)
                {
                    return View("Index", rules);
                }
                break;
            }
            var rule = Strategy.GetRule(ruleId);
            rule.ParentChangeId = string.IsNullOrEmpty(parentId) ? (Guid?)null : new Guid(parentId);
            Strategy.SaveRule(rule);
            return View("Index", RuleHelper.SortRules(Strategy.GetRuleBookRules(CurrentRuleBookId).ToArray()));
        }

        public ActionResult DeleteRule(int ruleId)
        {
            if (Strategy.GetRule(ruleId).RuleBookId == CurrentRuleBookId)
            {
                Strategy.DeleteRule(ruleId);
            }
            else
            {
                ViewBag.ErrorMessage = "Can't delete rule that is not an override.";
            }

            return View("Index", RuleHelper.SortRules(Strategy.GetRuleBookRules(CurrentRuleBookId).ToArray()));
        }

        public ActionResult Index(int? ruleBookId = null, int? id = null, string parentId = null)
        {
            if (ruleBookId != null)
            {
                CurrentRuleBookId = ruleBookId ?? 0;
                ViewBag.CurrentRuleBook = Strategy.GetRuleBook(CurrentRuleBookId).Name;
                ViewBag.CurrentRuleBookId = CurrentRuleBookId;
            }
            if (id != null)
            {
                return View("Rule", Strategy.GetRule(id ?? 0) ?? new RuleEngineRule("0", "NewRule")
                                                            {
                                                                ParentChangeId =
                                                                    string.IsNullOrEmpty(parentId)
                                                                        ? (Guid?)null
                                                                        : new Guid(parentId),
                                                                ChangeId = Guid.NewGuid().ToString(),
                                                                EffectiveDate = MissionControlDate,
                                                                Active = true
                                                            });
            }
            return View(RuleHelper.SortRules(Strategy.GetRuleBookRules(CurrentRuleBookId).ToArray()));
        }

        public ActionResult RuleExpression(int ruleId)
        {
            return Content(Strategy.GetRule(ruleId).RuleExpression);
        }

        public ActionResult SaveRule(RuleEngineRule model)
        {
            // Is this an override of the current rulebook already
            var forceNew = CurrentRuleBookId != model.RuleBookId;
            // Set the rulebook id to the the current rule book to override it
            model.RuleBookId = CurrentRuleBookId;
            // Save the rule
            Strategy.SaveRule(model, forceNew);
            // Return the result view
            return View("Index", RuleHelper.SortRules(Strategy.GetRuleBookRules(CurrentRuleBookId).ToArray()));
        }

        public ActionResult SaveRuleExpression(int ruleId, string expression)
        {
            var rule = Strategy.GetRule(ruleId);
            // Is this an override of the current rulebook already
            var forceNew = CurrentRuleBookId != rule.RuleBookId;
            // Set the rulebook id to the the current rule book to override it
            rule.RuleBookId = CurrentRuleBookId;
            // Apply the new expression
            rule.RuleExpression = expression;
            // Save the rule
            Strategy.SaveRule(rule, forceNew);
            //
            ViewBag.CurrentRuleBookId = CurrentRuleBookId;
            // Return the result view
            return View("Index", RuleHelper.SortRules(Strategy.GetRuleBookRules(CurrentRuleBookId).ToArray()));
        }

        public ActionResult StepIntoRule(string id)
        {
            StepInto = id;
            ViewBag.StepInto = id;
            return View("Index", RuleHelper.SortRules(Strategy.GetRuleBookRules(CurrentRuleBookId).ToArray()));
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var currentRuleBook = Strategy.GetRuleBook(CurrentRuleBookId, true);
            ViewBag.CurrentRuleBookEffectiveDate = currentRuleBook.EffectiveDate;
            ViewBag.CurrentRuleBookPoly = currentRuleBook.DerivedRuleBooks;
            ViewBag.CurrentRuleBook = Strategy.GetRuleBook(CurrentRuleBookId).Name;
            ViewBag.CurrentRuleBookId = CurrentRuleBookId;
        }
    }
}