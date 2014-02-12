using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using More.Application.BaseModel;
using More.Engine.BaseModel;
using More.Engine.CodeGen.Templates.Model;

namespace More.Engine.Compiling
{
    /// <summary>
    /// The class that really makes this rating engine come together in its conception.
    /// Responsible for grabbing all the data, generating the code and then compiling it
    /// into an assembly which can be then be used by the 'Rating Engine' dynamically.
    /// </summary>
    public class RatingEngineCompiler
    {
        #region Decorator Properties

        public IRulesEngineCodeProvider CodeProvider { get; set; }

        public RuleEngineCodeModel RuleEngineCodeModel { get; set; }

        //public bool IncludeDebug { get; set; }

        #endregion Decorator Properties

        public RatingEngineCompiler(RuleEngineCodeModel rulesEngineCodeModel)
            : this(new RulesEngineCodeProvider(), rulesEngineCodeModel)
        {
        }

        public RatingEngineCompiler(IRulesEngineCodeProvider codeProvider, RuleEngineCodeModel rulesEngineCodeModel)
        {
            RuleEngineCodeModel = rulesEngineCodeModel;
            CodeProvider = codeProvider;
            if (codeProvider == null)
                throw new Exception("Code Provider cannot be null");
        }

        #region Compiling And Assembling

        public static AppDomain RatingDomain { get; set; }

        public string Code
        {
            get { return CodeProvider.GetCode(RuleEngineCodeModel); }
        }

        public static string AssemblyNameByDate(DateTime time)
        {
            return "Rater_" + time.Day + "_" + time.Month + "_" + time.Year;
        }

        public static Assembly GetAssemblyFromFile(string filename)
        {
            if (RatingDomain != null)
            {
                AppDomain.Unload(RatingDomain);
                RatingDomain = null;
            }
            var ads = new AppDomainSetup
                          {
                              ShadowCopyFiles = "true",
                              PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath
                          };
            RatingDomain = AppDomain.CreateDomain("Carolina.RatingEngine", null, ads);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(p => p.FullName.StartsWith("Carolina.Library.Rat") || p.FullName.StartsWith("System")))
                RatingDomain.Load(assembly.FullName);
            //RatingDomain.Load(LoadFile(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,"Carolina.Rating.DLL")));
            var assemblyFile = LoadFile(filename);
            return RatingDomain.Load(assemblyFile);
        }

        public CompilerResults Build(CompilerParameters parameters = null)
        {
            return Build(Code, parameters);
        }

        private static byte[] LoadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs = null;
            return buffer;
        }

        private CompilerResults Build(string code, CompilerParameters parameters = null)
        {
            if (RatingDomain != null)
            {
                AppDomain.Unload(RatingDomain);
                RatingDomain = null;
            }

            var provider =
               new CSharpCodeProvider();
            ICodeCompiler compiler = provider.CreateCompiler();
            var compilerparams = parameters ?? new CompilerParameters()
                                                   {
                                                       GenerateExecutable = false,
                                                       GenerateInMemory = true
                                                   };

            compilerparams.ReferencedAssemblies.Add("System.dll");
            compilerparams.ReferencedAssemblies.Add("System.Core.dll");
            compilerparams.ReferencedAssemblies.Add("System.Data.dll");
            compilerparams.ReferencedAssemblies.Add("System.Xml.dll");
            compilerparams.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            compilerparams.ReferencedAssemblies.Add(this.GetType().Assembly.Location);
            compilerparams.ReferencedAssemblies.Add(typeof(IRulesEngineRepository).Assembly.Location);
            //compilerparams.ReferencedAssemblies.Add(typeof(SqlLookupTableRepository).Assembly.Location);

            CompilerResults results =
               compiler.CompileAssemblyFromSource(compilerparams, code);

            return results;
        }

        #endregion Compiling And Assembling
    }
}