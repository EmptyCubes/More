using System.Collections.Generic;
using NCalc;

namespace More.Engine.CodeGen.Templates.Model
{
    public class RulesEngineCodeContext
    {
        public ContextList<string> DeclaredVariables { get; set; }

        public bool IsExceptionClass { get; set; }

        public string LibraryName { get; set; }

        public List<string> Lookups { get; set; }

        public string[] Outputs { get; set; }

        public List<string> Variables { get; set; }

        protected Stack<string> ContextStack { get; set; }

        protected string CurrentContext
        {
            get
            {
                if (ContextStack.Count < 1) return "inputs";
                return ContextStack.Peek();
            }
        }

        public RulesEngineCodeContext(string libraryName)
        {
            LibraryName = libraryName;
            DeclaredVariables = new ContextList<string>();
            ContextStack = new Stack<string>();
            Lookups = new List<string>();
            Variables = new List<string>();
        }

        //protected string CurrentClass
        //{
        //    get { return _currentClass; }
        //    set { _currentClass = value; _declaredTableMethods.Add(value, new List<string>()); }
        //}

        //protected List<string> CurrentClassMethods
        //{
        //    get { return _declaredTableMethods[CurrentClass]; }
        //}
        public void EnterContext()
        {
            DeclaredVariables.PushContext();
            //ContextCount++;
        }

        public string GetValueString(object value)
        {
            double o;
            if (double.TryParse(value.ToString(), out o))
                return o.ToString();
            if (value is string)
            {
                return "\"" + value + "\".ToLower()";
            }
            return value.ToString();
        }

        public void LeaveContext()
        {
            DeclaredVariables.PopContext();
            //ContextCount--;
        }

        public string ToCSharpExpression(string expression)
        {
            return Expression.Compile(expression, true).ToString(new CSharpSerializationVisitor());
        }

        public string VarWhenNeeded(string variable)
        {
            return DeclaredVariables.Contains(variable) ? "" : "var ";
        }
    }
}