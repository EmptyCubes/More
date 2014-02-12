using System;
using System.Collections.Generic;
using NCalc.Domain;

namespace More.Engine.CodeGen
{
    /// <summary>
    /// This is a spagetti attempt at explaning in 'English' what a NCalc expression means
    /// </summary>
    public class ExplainSerializationVisitor : SerializationVisitor
    {
        public Dictionary<string, string> Values { get; set; }

        public void EncapsulateNoValue(string convertMethod, LogicalExpression logicalExpression)
        {
            //Result.AppendFormat("{0}(", convertMethod);
            if (logicalExpression != null)
                logicalExpression.Accept(this);
            //EncapsulateNoValue(logicalExpression);
            //Result.Append(")");
        }

        public override void Visit(BinaryExpression expression)
        {
            switch (expression.Type)
            {
                case BinaryExpressionType.And:
                    EncapsulateNoValue(expression.LeftExpression);
                    Result.Append("AND ");
                    EncapsulateNoValue(expression.RightExpression);
                    break;

                case BinaryExpressionType.Or:
                    EncapsulateNoValue(expression.LeftExpression);
                    Result.Append(" OR ");
                    EncapsulateNoValue(expression.RightExpression);
                    break;

                case BinaryExpressionType.Div:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("/ ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.Equal:
                    EncapsulateNoValue(expression.LeftExpression);
                    //Result.Append(".Equals(");
                    EncapsulateNoValue(expression.RightExpression);
                    // Result.Append(")");
                    break;

                case BinaryExpressionType.Greater:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("GREATER THAN ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.GreaterOrEqual:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("GREATER OR EQUAL TO ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.Lesser:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("LESS THAN ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.LesserOrEqual:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("LESS THAN OR EQUAL TO ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.Minus:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("- ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.Modulo:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("% ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.NotEqual:

                    EncapsulateNoValue(expression.LeftExpression);
                    Result.Append("IS NOT EQUAL TO ");
                    EncapsulateNoValue(expression.RightExpression);

                    break;

                case BinaryExpressionType.Plus:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("+ ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.Times:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("* ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.BitwiseAnd:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("& ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.BitwiseOr:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("| ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.BitwiseXOr:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("~ ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.LeftShift:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("<< ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.RightShift:
                    Result.Append(">> ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;
            }
        }

        public override void Visit(Identifier parameter)
        {
            var parts = parameter.Name.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1)
            {
                Result.Append("##" + parameter.Name + "##");
                //if (Values.ContainsKey(parameter.Name))
                //Result.Append(Values[parameter.Name]);
            }
            else
            {
                //if (Values.ContainsKey(parameter.Name))
                //Result.AppendFormat("GetContextResult({0} as List<object>, \"{1}\").ToArray()", parts[0], parts[1]);
            }
        }

        public override void Visit(ValueExpression expression)
        {
            Result.Append(expression);
        }
    }
}