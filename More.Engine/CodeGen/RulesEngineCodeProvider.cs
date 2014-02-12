using More.Engine.CodeGen.Templates;
using More.Engine.CodeGen.Templates.Model;

namespace More.Engine.BaseModel
{
    public class RulesEngineCodeProvider : IRulesEngineCodeProvider
    {
        public string GetCode(RuleEngineCodeModel model)
        {
            //var ruleBooks = decorator.GetRuleBooks().Select(
            //    p => new RuleBookTemplateModel(
            //             p.Name, decorator.GetRuleBookRules(Convert.ToInt32((string) p.Id), null).ToArray(),null
            //             )
            //    ).ToArray();

            //var model = new RuleEngineCodeModel()
            //                {
            //                    RuleBooks = ruleBooks,
            //                    BaseLookupTables =
            //                        decorator.DecorateTables(false,
            //                                                 ruleBooks.SelectMany(p=>p.Lookups).ToArray()).ToArray()
            //                };

            var codeGenerator = new RulesEngineCode()
                                    {
                                        Model = model,
                                        Context = new RulesEngineCodeContext("TESTRATER")
                                    };
            var result = codeGenerator.TransformText();
            return result;
        }
    }
}