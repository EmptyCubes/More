using System;
using System.Collections.Generic;
using System.Linq;
using More.Engine.CodeGen;
using More.Engine.Model;

namespace More.Engine.Compiling
{
    public static class RuleHelper
    {
        public static IEnumerable<RuleEngineRule> SortRules(RuleEngineRule[] rules)
        {
            var ratingRule = new RuleEngineRule();
            ratingRule.Children.AddRange(rules);
            try
            {
                SortRules(ratingRule);
            }
            catch (Exception ex)
            {
                return rules;
            }
            return ratingRule.Children;
        }

        public static void SortRules(RuleEngineRule ratingRule)
        {
            var children = ratingRule.Children.ToArray();

            var variableContext = new ContextList<string>();
            variableContext.PushContext();

            var outputs = children.SelectMany(p => p.Outputs).ToArray();
            var variables = children.SelectMany(p => p.AllInputs).Except(outputs).ToList();

            foreach (var variable in variables)
            {
                variableContext.Add(variable);
            }

            SortRules(ratingRule, variableContext);

            variableContext.PopContext();
        }

        public static void SortRules(RuleEngineRule rule, ContextList<string> variables)
        {
            //var childList = rule.Children.ToArray();
            // Clear the children
            //rule.Children.Clear();
            if (rule.Children.Count > 0)
            {
                variables.PushContext();
                // Push a new variable context

                rule.Children = new RulesCollection(RuleEngineRule.SortedRules(rule.Children, variables.All.ToArray()), rule);
                foreach (var child in rule.Children)
                {
                    SortRules(child, variables);
                }
                variables.PopContext();
            }
            variables.Add(rule.Name);
        }
    }
}