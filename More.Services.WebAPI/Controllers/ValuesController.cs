using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using More.Application;
using More.Application.Json;
using More.Engine;
using More.Engine.Model;
using More.Services.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace More.Services.WebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        //public static string WriteData(IEnumerable<RuleBookTraceInformation> trace)
        //{
        //    var sb = new StringBuilder();
        //    var writer = new JsonTextWriter(new StringWriter(sb));

        //    writer.WriteStartObject();
        //    WriteData(writer, trace);
        //    writer.WriteEndObject();
        //}

        //public RuleBookTraceInformation[] Execute(DateTime effectiveDate, string ruleBookName, Dictionary<string, object> inputs)
        //{
        //    //MoreApplicationFactory.GetCompiler(effectiveDate);
        //    var rulesEngine = new RulesEngine();
        //    var factory = rulesEngine.GetFactory(MoreApplicationFactory.GetAssemblyName(effectiveDate));
        //    if (factory == null)
        //    {
        //        throw new Exception(
        //            "Either the assembly was not found.  Or it was compiled incorrectly. Recompile and try again.");
        //    }
        //    var ruleBook = factory.GetRuleBook(ruleBookName);

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        //    if (ruleBook == null)
        //    {
        //        throw new Exception("Rule book not found.");
        //    }
        //    var result = ruleBook.Execute(inputs);
        //    if (result.Errors.Count() > 0)
        //    {
        //        var error = result.Errors.First();
        //        throw new Exception(string.Format("{0} - {1}", error.Message, error.RuleDescription));
        //    }
        //    return result.TraceInformation.ToArray();
        //}
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value2" };
        }

        // GET api/values/5
        public string Get(RulesExecuteModel model)
        {
            return "value";
        }

        // POST api/values
        public object Post(RulesExecuteModel model)
        {
            // MoreApplicationFactory.GetCompiler(model.EffectiveDate);
            try
            {
                var inputs = JsonHelper.FromJson(model.Data);
                var rulesEngine = new RulesEngine();
                var assemblyEffectiveDate = MoreApplicationFactory.GetAssemblyEffectiveDate(model.EffectiveDate);
                var factory = rulesEngine.GetFactory(
                    MoreApplicationFactory.GetAssemblyName(assemblyEffectiveDate),
                    MoreApplicationFactory.GetLookupRepository(assemblyEffectiveDate)
                    );
                if (factory == null)
                {
                    return
                        "Either the assembly was not found.  Or it was compiled incorrectly. Recompile and try again.";
                }
                var ruleBook = factory.GetRuleBook(model.RuleBook);

                if (ruleBook == null)
                {
                    return "Rule book not found.";
                }
                var result = ruleBook.Execute(inputs);
                if (result.Errors.Any())
                {
                    var error = result.Errors.First();
                    return string.Format("{0} - {1}", error.Message, error.RuleDescription);
                }
                if (model.Items != null && model.Items.Length > 0 && model.Items.First() != "null")
                {
                    return model.Items.ToDictionary(item => item, result.Find);
                }
                return JsonHelper.GetJsonTrace(result.Results);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // PUT api/values/5
        public void Put(int id, string value)
        {
        }
    }
}