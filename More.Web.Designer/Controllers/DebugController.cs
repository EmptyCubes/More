using System.Collections.Generic;
using System.Web.Mvc;
using More.Application;
using More.Engine;

namespace More.Web.Designer.Controllers
{
    public class DebugController : BaseController
    {
        //
        // GET: /Debug/

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult TestForm(string ruleBookName)
        //{
        //    ViewBag.RuleBookName = ruleBookName;
        //    MoreApplicationFactory.GetCompiler(MissionControlDate);
        //    var rulesEngine = new RulesEngine();
        //    var factory = rulesEngine.GetFactory(MoreApplicationFactory.GetAssemblyName(MissionControlDate), TableStrategy);
        //    var ruleBook = factory.GetRuleBook(ruleBookName);
        //    if (ruleBook == null)
        //    {
        //        return View("ErrorMessage", (object)"Rule book not found");
        //    }
        //    return View(ruleBook.GetInputRequirements());
        //}

        public ActionResult TestRuleBook(string ruleBookName)
        {
            ViewBag.RuleBookName = ruleBookName;
            MoreApplicationFactory.GetCompiler(MissionControlDate);
            var rulesEngine = new RulesEngine();
            var factory = rulesEngine.GetFactory(MoreApplicationFactory.GetAssemblyName(MissionControlDate), TableStrategy);
            var ruleBook = factory.GetRuleBook(ruleBookName);

            var ruleBookRequirements = ruleBook.GetInputRequirements();
            var inputs = new Dictionary<string, object>();

            foreach (var ruleBookRequirement in ruleBookRequirements)
            {
                inputs.Add(ruleBookRequirement.Name, Request[ruleBookRequirement.Name]);
            }

            return Json(ruleBook.Execute(inputs), JsonRequestBehavior.AllowGet);
        }
    }
}