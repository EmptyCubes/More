//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using More.Application;
//using More.Application.Entity;
//using More.Application.Json;
//using Rules12252015;

//namespace More.UnitTests
//{
//    [TestClass]
//    public class EngineRepositoryTests
//    {
//        [TestMethod]
//        public void TestGetBaseRulebooks()
//        {
//            var date = new DateTime(2015, 12, 25);
//            var factory = MoreApplicationFactory.GetEngineRepository(date) as EntityRulesEngineRepository;
//            var rules = factory.GetBaseRuleBooks(20).ToArray();
//            foreach (var rule in rules)
//            {
//                Console.WriteLine("{0}", rule.Name);
//            }
//            Assert.IsTrue(rules.Length == 2);
//            Assert.IsTrue(rules[0].Name == "Liability_TN");
//            Assert.IsTrue(rules[1].Name == "Liability_CW");
//        }

//        [TestMethod]
//        public void TestGetBaseRulebookRules()
//        {
//            var date = new DateTime(2015, 12, 25);
//            var factory = MoreApplicationFactory.GetEngineRepository(date) as EntityRulesEngineRepository;
//            var rules = factory.GetBaseRuleBookRules(20).ToArray();
//            foreach (var rule in rules)
//            {
//                Console.WriteLine("{0}: {1}", rule.Name, rule.RuleExpression);
//            }
//        }
//        [TestMethod]
//        public void TestLookupTable()
//        {
//            var crb = new CommonRuleBook();
//            Assert.AreEqual(crb.CCI_AgeTier("TIER", 17), 5);
//            Assert.AreEqual(crb.CCI_AgeTier("TIER", 28), 4);
//            Assert.AreEqual(crb.CCI_AgeTier("TIER", 35), 3);
//            Assert.AreEqual(crb.CCI_AgeTier("TIER", 40), 2);
//            Assert.AreEqual(crb.CCI_AgeTier("TIER", 54), 1);
//        }
//        [TestMethod]
//        public void TestExecute()
//        {
//            CompiledRuleBookFactory factory = new CompiledRuleBookFactory();
//            var ruleBook = factory.GetRuleBook("Rating_CW");
//            var result = ruleBook.Execute(new Dictionary<string, object>
//            {
//            });
//            Console.WriteLine(JsonHelper.WriteData(result.Results));
//            var final = result.Path("Coverages", "Liability", "Root")["Final"];
//            Assert.IsTrue(Convert.ToDecimal(final) == 25);

//            ruleBook = factory.GetRuleBook("Rating_TN");
//            result = ruleBook.Execute(new Dictionary<string, object> { });
//            final = result.Path("Coverages", "Liability", "Root")["Final"];

//            Assert.IsTrue(Convert.ToDecimal(final) == 36);
//            Console.WriteLine(JsonHelper.WriteData(result.Results));
//        }
//    }
//}