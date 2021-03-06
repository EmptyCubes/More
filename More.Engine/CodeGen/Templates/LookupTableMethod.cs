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
    
    #line 1 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public partial class LookupTableMethod : RatingEngineCodeTemplateBase<More.Engine.Model.LookupTable>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            
            #line 9 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
 
	string[] columnVars = Model.Keys.Select(p => "_" + StringUtils.StripAllButChars(p.Name)).ToArray(); 
	var isOverride = Context.IsExceptionClass;


            
            #line default
            #line hidden
            this.Write("\r\n\r\n\t\tpublic ");
            
            #line 16 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(isOverride ? "override" : "virtual"));
            
            #line default
            #line hidden
            this.Write(" object ");
            
            #line 16 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("(string column, object ");
            
            #line 16 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", object ", columnVars)));
            
            #line default
            #line hidden
            this.Write(") {\r\n\t\t\treturn Lookup(\"");
            
            #line 17 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Properties.Schema.ToString()));
            
            #line default
            #line hidden
            this.Write("\", \"");
            
            #line 17 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("()\", column, ");
            
            #line 17 "C:\CodePlex\more\More.Engine\CodeGen\Templates\LookupTableMethod.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", columnVars)));
            
            #line default
            #line hidden
            this.Write(");\r\n\t\t}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
