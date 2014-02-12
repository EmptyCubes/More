using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using More.Engine.Evaluation;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new CompiledRuleBookBase();
            Console.WriteLine(a.LessThanOrEqual("2", "-0.5"));
            Console.ReadLine();
        }
    }
}
