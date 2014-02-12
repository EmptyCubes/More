using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;
using More.Application;
using More.Application.BaseModel;
using More.Engine.Model;

namespace More.Web.Designer.Controllers
{
    public class AssembliesController : BaseController
    {
        //
        // GET: /Assemblies/

        public ActionResult AddAssembly(DateTime? effectiveDate = null)
        {
            if (effectiveDate == null)
            {
                return View();
            }

            Strategy.SaveRuleAssembly(MoreApplicationFactory.GetAssemblyName(effectiveDate.Value), effectiveDate.Value);

            return View("Index", Strategy.GetRatingAssemblies());
        }

        public ActionResult Compile(string effectiveDate = null)
        {
            try
            {
                var date = string.IsNullOrEmpty(effectiveDate) ? MissionControlDate : Convert.ToDateTime(effectiveDate);
                var assemblyName = MoreApplicationFactory.GetAssemblyName(date);
                var assemblyFileName = Path.Combine(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath, assemblyName + ".dll");

                var compiler = MoreApplicationFactory.GetCompiler(date);

                var results = compiler.Build(
                                             new CompilerParameters()
                                             {
                                                 GenerateInMemory = false,
                                                 GenerateExecutable = false,
                                                 OutputAssembly = assemblyFileName,
                                             });

                if (results.Errors.HasErrors)
                {
                    var lstErrors = new List<CompilerErrorModel>();
                    foreach (CompilerError error in results.Errors)
                        lstErrors.Add(new CompilerErrorModel(error.ErrorText, error.Line, error.Column));
                    return View("Errors", lstErrors);
                }

                Strategy.SaveRuleAssembly(assemblyName, date);
            }
            catch (Exception ex)
            {
                return View("Errors", new[] { new CompilerErrorModel(ex.Message, 0, 0) });
            }
            return View("CompilerSuccess");
        }

        public ActionResult ExportCS()
        {
            var compiler = MoreApplicationFactory.GetCompiler(MissionControlDate);
            var path = Path.Combine(WebConfigurationManager.AppSettings["PublishLocation"], "CompiledCS.cs");
            System.IO.File.WriteAllText(path, compiler.Code);
            return File(path, "text/plain");
        }

        public ActionResult Index()
        {
            return View(Strategy.GetRatingAssemblies());
        }

        public ActionResult PublishAssembly(string effectiveDate = null)
        {
            var date = string.IsNullOrEmpty(effectiveDate) ? MissionControlDate : Convert.ToDateTime(effectiveDate);
            var assemblyName = MoreApplicationFactory.GetAssemblyName(date);
            var assemblyFileName = Path.Combine(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath, assemblyName + ".dll");
            Strategy.PublishRatingAssembly(date);
            var copyToDirectory = WebConfigurationManager.AppSettings["PublishLocation"];
            var copyToDirectories = copyToDirectory.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var dir in copyToDirectories)
            {
                try
                {
                    if (Directory.Exists(dir))
                    {
                        System.IO.File.Copy(assemblyFileName, Path.Combine(dir, assemblyName + ".dll"));
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("Couldn't find publish directory " + dir);
                }
            }
            return View("Index", Strategy.GetRatingAssemblies());
        }

        public ActionResult SetAssembly(string effectiveDate = null)
        {
            var date = string.IsNullOrEmpty(effectiveDate) ? MissionControlDate : Convert.ToDateTime(effectiveDate);
            MissionControlDate = Convert.ToDateTime(date);
            ViewBag.MissionControlDate = MissionControlDate;
            return RedirectToAction("Index");
        }

        public ActionResult ShowCode()
        {
            var compiler = MoreApplicationFactory.GetCompiler(MissionControlDate);
            return Content(compiler.Code);
        }
    }
}