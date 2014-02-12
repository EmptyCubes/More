using System.Linq;
using System.Web.Mvc;
using More.Engine.Model;

namespace More.Web.Designer.Controllers
{
    public class IntellisenseController : BaseController
    {
        public ActionResult Methods()
        {
            //var ratingDecorator = TableStrategy as IRatingDecorator;
            //if (ratingDecorator == null)
            //{
            //    return Json(new[] { "Couldn't load RateTable methods" });
            //}
            //return Json(ratingDecorator.DecorateTables(true).OrderBy(p => p.Name).Select(
            //    p => new { name = p.Name, value = p.Description, description = p.Description, type = "method" }),
            //    JsonRequestBehavior.AllowGet);
            return new EmptyResult();
        }

        //
        // GET: /RatingEngine/Intellisense/
        public ActionResult RuleVars(string id)
        {
            var result = new object[] { };
            if (id != null)
            {
                var root = new RuleEngineRule();
                root.Children.AddRange(Strategy.GetRuleBookRules(CurrentRuleBookId, null));

                var item = root.Find(id);
                if (item != null)
                {
                    result = result.Union(item.AccessibleVars.OrderBy(p => p)
                        .Select(p => new
                    {
                        name = p,
                        value = p,
                        type = "variable",
                        description = p
                    })).ToArray();
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Variables(string id)
        {
            var result = new object[] { };
            if (id != null)
            {
                var root = new RuleEngineRule();
                root.Children.AddRange(Strategy.GetRuleBookRules(CurrentRuleBookId, null));

                var item = root.Find(id);
                if (item != null)
                {
                    result = result.Union(item.AccessibleVars.OrderBy(p => p).Select(p => new
                                                                          {
                                                                              name = p,
                                                                              value = p,
                                                                              type = "variable",
                                                                              description = p
                                                                          })).ToArray();
                }
            }
            // TODO Find out how this fits in if we arn't using the configuration part anymore
            //result = result.Union(ConfigExpressionTree.GetAll().OrderBy(p => p.Name)
            //    .Select(p => new
            //            {
            //                name = p.Display,
            //                value = p.Name,
            //                type = "variable",
            //                description = p.Name + "( " + p.Display + " )"
            //            }).ToArray().Distinct().ToArray()).ToArray();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}