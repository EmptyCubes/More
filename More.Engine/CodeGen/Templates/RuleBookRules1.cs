﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 10.0.0.0
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
    
    
    #line 1 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
    public partial class RuleBookRules : RatingEngineCodeTemplateBase<System.Collections.Generic.IEnumerable<RuleEngineRule>>
    {
        public override string TransformText()
        {
            
            #line 9 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 foreach (var rule in Model) { 
            
            #line default
            #line hidden
            this.Write("\t");
            
            #line 10 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 if (rule.Children.Count > 0) { 
            
            #line default
            #line hidden
            this.Write(" \r\n\t\t");
            
            #line 11 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 if (!string.IsNullOrEmpty(rule.LoopContext)) { 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 12 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
WriteVariableDecleration(rule.Name);
            
            #line default
            #line hidden
            this.Write(" = GetInput(\"");
            
            #line 12 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.LoopContext));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t");
            
            #line 13 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\t\tif (");
            
            #line 14 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Context.ToCSharpExpression(rule.RuleExpression)));
            
            #line default
            #line hidden
            this.Write(") {\r\n\t\tConditionEntered(\"");
            
            #line 15 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t");
            
            #line 16 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 EnterContext(); 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 17 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 if (!string.IsNullOrEmpty(rule.LoopContext)) {  var itemName = rule.Name + "Item"; 
            
            #line default
            #line hidden
            this.Write("\t\t\tLoopEntered(\"");
            
            #line 18 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\",\"");
            
            #line 18 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.LoopContext));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t\t");
            
            #line 19 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
WriteVariableDecleration(rule.LoopContext);
            
            #line default
            #line hidden
            this.Write(" = GetInput(\"");
            
            #line 19 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.LoopContext));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t\t\r\n\t\t\tvar ");
            
            #line 21 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(itemName));
            
            #line default
            #line hidden
            this.Write("Index = 0;\r\n\t\t\tforeach (IDictionary<string,object> ");
            
            #line 22 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(itemName));
            
            #line default
            #line hidden
            this.Write(" in (IList)");
            
            #line 22 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.LoopContext));
            
            #line default
            #line hidden
            this.Write(") {\r\n\t\t\t\tLoopItemEntered(");
            
            #line 23 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(itemName));
            
            #line default
            #line hidden
            this.Write("Index, \"");
            
            #line 23 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\",\"");
            
            #line 23 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.LoopContext));
            
            #line default
            #line hidden
            this.Write("\", ");
            
            #line 23 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(itemName));
            
            #line default
            #line hidden
            this.Write(");\r\n\t\t\t\t");
            
            #line 24 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 EnterContext(); 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 25 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 var vars = rule.ChildrenExternalInputs.Where(x=>!Context.DeclaredVariables.Contains(x)); 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 26 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 foreach (var v in vars) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 27 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
WriteVariableDecleration(v);
            
            #line default
            #line hidden
            this.Write(" = GetInput(\"");
            
            #line 27 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(v));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t\t\t");
            
            #line 28 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
}
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 29 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 WriteTemplate<RuleBookRules,IEnumerable<RuleEngineRule>>( rule.Children); 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 30 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 LeaveContext(); 
            
            #line default
            #line hidden
            this.Write("\t\t\t\tLoopItemExited(");
            
            #line 31 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(itemName));
            
            #line default
            #line hidden
            this.Write("Index, \"");
            
            #line 31 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\",\"");
            
            #line 31 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.LoopContext));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t\t\t");
            
            #line 32 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(itemName));
            
            #line default
            #line hidden
            this.Write("Index++;\r\n\t\t\t}\r\n\t\t\tLoopExited(\"");
            
            #line 34 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\",\"");
            
            #line 34 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.LoopContext));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t");
            
            #line 35 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
} else { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 36 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 var vars = rule.ChildrenExternalInputs.Where(x=>!Context.DeclaredVariables.Contains(x)); 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 37 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 foreach (var v in vars) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t\t");
            
            #line 38 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
WriteVariableDecleration(v);
            
            #line default
            #line hidden
            this.Write(" = GetInput(\"");
            
            #line 38 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(v));
            
            #line default
            #line hidden
            this.Write("\");\t\t\t\r\n\t\t\t\t");
            
            #line 39 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
}
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 40 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 WriteTemplate<RuleBookRules,IEnumerable<RuleEngineRule>>( rule.Children); 
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 41 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
}
            
            #line default
            #line hidden
            this.Write("\t\t");
            
            #line 42 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 LeaveContext(); 
            
            #line default
            #line hidden
            this.Write("\t\tConditionExited(\"");
            
            #line 43 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n\t\t}\r\n\t");
            
            #line 45 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
} else { 
            
            #line default
            #line hidden
            this.Write("\t\t\r\n\t    ");
            
            #line 47 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
WriteVariableDecleration(rule.Name);
            
            #line default
            #line hidden
            this.Write(" =\r\n\t\t\tStatementExecuted(\"");
            
            #line 48 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Id));
            
            #line default
            #line hidden
            this.Write("\", \"");
            
            #line 48 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\", \"");
            
            #line 48 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\",");
            
            #line 48 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(StringUtils.ToLiteral(rule.RuleExpression)));
            
            #line default
            #line hidden
            this.Write(", \r\n\t\t\t\t ()=>{ return ");
            
            #line 49 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Context.ToCSharpExpression(rule.RuleExpression)));
            
            #line default
            #line hidden
            this.Write("; } // <------- ");
            
            #line 49 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(rule.Name));
            
            #line default
            #line hidden
            this.Write("\r\n\t\t\t\t );\r\n\t");
            
            #line 51 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
}
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 53 "C:\CodePlex\more\More.Engine\CodeGen\Templates\RuleBookRules.tt"
 } 
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
