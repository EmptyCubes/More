using System.Collections.Generic;
using NCalc.Domain;

namespace More.Engine.CodeGen
{
    /// <summary>
    /// This is a simple little variable/method extraction helper class
    /// </summary>
    public class NCalcVariableExtractionVisitor : LogicalExpressionVisitor
    {
        public List<string> Methods { get; set; }

        public List<string> Variables { get; set; }

        public NCalcVariableExtractionVisitor()
        {
            Variables = new List<string>();
            Methods = new List<string>();
        }

        public override void Visit(LogicalExpression expression)
        {
            //base.Visit(expression);
        }

        public override void Visit(TernaryExpression expression)
        {
            //base.Visit(expression);
            expression.LeftExpression.Accept(this);
            expression.MiddleExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(BinaryExpression expression)
        {
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(UnaryExpression expression)
        {
            expression.Expression.Accept(this);
        }

        public override void Visit(ValueExpression expression)
        {
            // base.Visit(expression);
        }

        public override void Visit(Function function)
        {
            Methods.Add(function.Identifier.Name);
            foreach (var rule in function.Expressions)
                rule.Accept(this);
        }

        public override void Visit(Identifier function)
        {
            Variables.Add(function.Name.Split(":".ToCharArray())[0]);
        }
    }
}