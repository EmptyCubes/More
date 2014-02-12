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
    
    #line 1 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public partial class RuleBook : RatingEngineCodeTemplateBase<RuleBookTemplateModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\npublic class ");
            
            #line 10 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 10 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.BaseClassName));
            
            #line default
            #line hidden
            this.Write(" {\r\n\t\t public override IEnumerable<InputRequirement> GetInputRequirements() {\r\n\t\t" +
                    "\t");
            
            #line 12 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 if (Model.Rules != null) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 13 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 var externalInputs = Model.Rules.SelectMany(p=>p.ExternalInputs).ToArray(); 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 14 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
if (externalInputs.Length > 0) {
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 15 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 foreach (var externalInput in externalInputs) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t yield return new InputRequirement() { Name = \"");
            
            #line 16 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(externalInput));
            
            #line default
            #line hidden
            this.Write("\" };\r\n\t\t\t\t");
            
            #line 17 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 18 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 } else {
            
            #line default
            #line hidden
            this.Write("\t\t\t\tyield break;\r\n\t\t\t");
            
            #line 20 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
}
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 21 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tyield break;\r\n\t\t\t");
            
            #line 23 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\t }\r\n\t\tprotected override RuleBookResult InternalExecute(Dictionary<string,objec" +
                    "t> inputs) {\r\n\t\t   ");
            
            #line 26 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 Context.DeclaredVariables.PushContext(); 
            
            #line default
            #line hidden
            
            #line 27 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 if (Model.Rules != null) WriteTemplate<RuleBookRules,IEnumerable<RuleEngineRule>>( Model.Rules ); 
            
            #line default
            #line hidden
            this.Write("\t\t   ");
            
            #line 28 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBook.tt"
 Context.DeclaredVariables.PopContext(); 
            
            #line default
            #line hidden
            this.Write("\t\t    return Result;\r\n\t\t}\r\n\r\n\t\t\r\n\r\n\r\n\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
