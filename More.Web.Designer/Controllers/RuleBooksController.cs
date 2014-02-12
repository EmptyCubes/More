using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using More.Engine.Compiling;

namespace More.Web.Designer.Controllers
{
    public class RuleBooksController : BaseController
    {
        //
        // GET: /RuleBooks/

        public ActionResult DeleteRuleBook(int id)
        {
            Strategy.DeleteRuleBook(id);
            return View("Index", Strategy.GetRuleBooks());
        }

        public ActionResult Index(int? id = null)
        {
            if (id != null)
            {
                ViewBag.RuleBooks = GetRuleBooksList(Strategy.GetRuleBooks().ToArray());
                return View("RuleBook", Strategy.GetRuleBook(id ?? 0));
            }
            return View(Strategy.GetRuleBooks());
        }

        public ActionResult SaveRuleBook(RuleBookModel model)
        {
            Strategy.SaveRuleBook(model);
            return View("Index", Strategy.GetRuleBooks());
        }

        private IEnumerable<SelectListItem> GetRuleBooksList(IEnumerable<RuleBookModel> books, int indent = 0)
        {
            if (indent == 0)
            {
                yield return new SelectListItem()
                {
                    Selected = false,
                    Text = "[No Base]",
                    Value = null
                };
            }
            foreach (var ruleBook in books)
            {
                yield return new SelectListItem()
                {
                    Selected = false,
                    Text = ">".PadLeft(indent * 4, '-') + ruleBook.Name,
                    Value = ruleBook.ChangeId
                };
                var derived = GetRuleBooksList(ruleBook.DerivedRuleBooks, indent + 1).ToArray();
                foreach (var item in derived)
                {
                    yield return item;
                }
            }
        }
    }
}