using System;
using NCalc.Domain;
using ValueType = NCalc.Domain.ValueType;

namespace More.Engine.CodeGen
{
    /// <summary>
    /// This class converts a NCalc Expression into a C# Expression.
    /// NOTE: This one visitor inparticular is somewhat coupled with how the rating engine works.
    /// NOTE: This is only because of the "GetContextResult" code in the Visit(Identifier parameter) method
    /// </summary>
    public class CSharpSerializationVisitor : SerializationVisitor
    {
        public void EncapsulateNoValue(string convertMethod, LogicalExpression logicalExpression)
        {
            Result.AppendFormat("{0}(", convertMethod);
            EncapsulateNoValue(logicalExpression);
            Result.Append(")");
        }

        public override void Visit(BinaryExpression expression)
        {
            switch (expression.Type)
            {
                case BinaryExpressionType.And:

                    Result.Append("Convert.ToBoolean(");
                    EncapsulateNoValue(expression.LeftExpression);
                    Result.Append(")");
                    Result.Append("&& ");
                    Result.Append("Convert.ToBoolean(");
                    EncapsulateNoValue(expression.RightExpression);
                    Result.Append(")");
                    break;

                case BinaryExpressionType.Or:
                    Result.Append("Convert.ToBoolean(");
                    EncapsulateNoValue(expression.LeftExpression);
                    Result.Append(")");
                    Result.Append("|| ");
                    Result.Append("Convert.ToBoolean(");
                    EncapsulateNoValue(expression.RightExpression);
                    Result.Append(")");
                    break;

                case BinaryExpressionType.Div:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("/ ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.Equal:
                    EncapsulateNoValue(expression.LeftExpression);
                    Result.Append(".Equals(");
                    EncapsulateNoValue(expression.RightExpression);
                    Result.Append(")");
                    break;

                case BinaryExpressionType.Greater:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("> ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.GreaterOrEqual:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append(">= ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.Lesser:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("< ");
                    EncapsulateNoValue("Convert.ToDouble", expression.RightExpression);
                    break;

                case BinaryExpressionType.LesserOrEqual:
                    EncapsulateNoValue("Convert.ToDouble", expression.LeftExpression);
                    Result.Append("<= ");
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
                    Result.Append("!(");
                    EncapsulateNoValue(expression.LeftExpression);
                    Result.Append(".Equals(");
                    EncapsulateNoValue(expression.RightExpression);
                    Result.Append("))");
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

        public override void Visit(TernaryExpression expression)
        {
            Result.Append("(bool)");
            EncapsulateNoValue(expression.LeftExpression);

            Result.Append("? ");

            EncapsulateNoValue(expression.MiddleExpression);

            Result.Append(": ");

            EncapsulateNoValue(expression.RightExpression);
        }

        public override void Visit(Identifier parameter)
        {
            var parts = parameter.Name.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1)
            {
                Result.Append(parameter.Name);
            }
            else
            {
                Result.AppendFormat("GetInput(\"{0}\")", string.Join("\", \"", parts));
            }
        }

        public override void Visit(ValueExpression expression)
        {
            if (expression.Type == NCalc.Domain.ValueType.Integer)
            {
                base.Visit(expression);
            }
            else
                if (expression.Type == ValueType.Float)
                {
                    Result.Append("Convert.ToDouble(");
                    base.Visit(expression);
                    Result.Append(")");
                }
                else
                {
                    if (expression.Type == ValueType.String)
                    {
                        Result.Append(" \"").Append(expression.Value.ToString()).Append("\" ");
                    }
                    else
                        if (expression.Type == ValueType.Boolean)
                        {
                            Result.Append(expression.Value.ToString().ToLower()).Append(" ");
                        }
                        else
                        {
                            base.Visit(expression);
                        }
                }
        }
    }
}