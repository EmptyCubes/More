using System.CodeDom;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace More.Engine.CodeGen
{
    public class StringUtils
    {
        public static string StripAllButChars(string str)
        {
            var sb = new StringBuilder();
            var matches = Regex.Matches(str, @"[a-zA-Z]");
            foreach (Match match in matches)
            {
                sb.Append(match.Value);
            }
            return sb.ToString();
        }

        public static string StripAllButNumbers(string str)
        {
            var sb = new StringBuilder();
            var matches = Regex.Matches(str, @"[0-9]");
            foreach (Match match in matches)
            {
                sb.Append(match.Value);
            }
            return sb.ToString();
        }

        public static string ToLiteral(string input)
        {
            var writer = new StringWriter();
            var provider = new CSharpCodeProvider();
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.GetStringBuilder().ToString();
        }
    }
}