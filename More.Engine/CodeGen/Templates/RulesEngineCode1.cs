﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 11.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace More.Engine.CodeGen.Templates
{
    using System;
    using System.Xml;
    using System.Collections.Generic;
    using System.Linq;
    using More.Engine;
    using More.Engine.Model;
    using More.Engine.CodeGen.Templates.Model;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public partial class RulesEngineCode : RatingEngineCodeTemplateBase<RuleEngineCodeModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections;\r\nusing System.Collections.Generic;\r\nusin" +
                    "g System.Linq;\r\nusing More.Engine;\r\nusing More.Engine.Evaluation;\r\nusing More.En" +
                    "gine.Model;\r\nnamespace ");
            
            #line 16 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Namespace));
            
            #line default
            #line hidden
            this.Write(" {\r\npublic class CompiledRuleBookFactory : CompiledRuleBookFactoryBase {\r\n\tpublic" +
                    " override DateTime EffectiveDate {\r\n\t\tget {\r\n\t\t\treturn _effectiveDate ?? (_effec" +
                    "tiveDate = new DateTime(");
            
            #line 20 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.EffectiveDate.ToString()));
            
            #line default
            #line hidden
            this.Write("));\r\n\t\t}\r\n\t}\r\n\tpublic override CompiledRuleBookBase GetRuleBook(string name) {\r\n\t" +
                    "\t");
            
            #line 24 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
 Context.DeclaredVariables.PushContext(); 
            
            #line default
            #line hidden
            this.Write("\r\n\t\t");
            
            #line 26 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
 foreach (var ruleBook in Model.RuleBooks) { 
            
            #line default
            #line hidden
            this.Write("\t\t\tif (name.Equals(\"");
            
            #line 27 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ruleBook.Name));
            
            #line default
            #line hidden
            this.Write("\")) {\r\n\t\t\t\treturn new ");
            
            #line 28 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ruleBook.Name));
            
            #line default
            #line hidden
            this.Write("RuleBook() { Factory = this };\r\n\t\t\t}\r\n\t\t");
            
            #line 30 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 31 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
 Context.DeclaredVariables.PopContext(); 
            
            #line default
            #line hidden
            this.Write("\t\treturn null;\r\n\t}\r\n}\r\n\r\n");
            
            #line 36 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
 foreach (var ruleBook in Model.RuleBooks) { WriteTemplate<RuleBook, RuleBookTemplateModel>(ruleBook); } 
            
            #line default
            #line hidden
            this.Write(@" 

	public class CommonRuleBook : CompiledRuleBookBase {
		public override IEnumerable<InputRequirement> GetInputRequirements() {
			yield break;
		}
		protected override RuleBookResult InternalExecute(Dictionary<string,object> inputs) {
			return Result;
		}
            
            #line 45 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
 foreach (var ruleBook in Model.RuleBooks) { 
            
            #line default
            #line hidden
            this.Write("\r\t\tpublic Dictionary<string,object> ");
            
            #line 45 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ruleBook.Name));
            
            #line default
            #line hidden
            this.Write("(params object[] args) {\r\t\t\treturn Execute(\"");
            
            #line 45 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ruleBook.Name));
            
            #line default
            #line hidden
            this.Write("\",args);\r\t\t}\r\t\t");
            
            #line 45 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\r\t\t#region LookupMethods\r\n\t\t\t\r\n\t\t");
            
            #line 47 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
if (Model.BaseLookupTables != null) {
			foreach (var table in Model.BaseLookupTables) { 
            
            #line default
            #line hidden
            this.Write(" \r\n");
            
            #line 49 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
WriteTemplate<LookupTableMethod, LookupTable>(table); 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 50 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RulesEngineCode.tt"
} 
		}
            
            #line default
            #line hidden
            this.Write("\t\t\t\r\t\t#endregion\r}\r\r\n\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}