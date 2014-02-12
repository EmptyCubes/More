using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using More.Application.BaseModel;
using More.Engine.Evaluation;

namespace More.Engine
{
    /// <summary>
    /// The rating engine WOOT WOOT! Its primary role is to keep track of the rating assemblies
    /// including the domain at which these assemblies are executed under
    /// </summary>
    public class RulesEngine
    {
        #region Constructors (1)

        public static AppDomain RatingDomain { get; set; }

        public IEnumerable<Assembly> Assemblies
        {
            get { return RatingDomain.GetAssemblies(); }
        }

        public RulesEngine()
        {
            if (RatingDomain != null)
                Flush();
            else
                Initialize();
        }

        #endregion Constructors

        #region Properties (3)
        #endregion Properties

        #region Methods (3)

        // Public Methods (2) 

        public void Flush()
        {
            AppDomain.Unload(RatingDomain);
            Initialize();
        }

        public CompiledRuleBookFactoryBase GetFactory(string name, ILookupTablesRepository lookupRepository)
        {
            try
            {
                RatingDomain.Load(name);

                var rater = (CompiledRuleBookFactoryBase)RatingDomain.CreateInstanceAndUnwrap(name, name + ".CompiledRuleBookFactory");
                rater.LookupTablesRepository = lookupRepository;
                return rater;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Private Methods (1) 

        private void Initialize()
        {
            var setup = new AppDomainSetup
                            {
                                ApplicationName = "MoreEngine",
                                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                                PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath,
                                CachePath = AppDomain.CurrentDomain.SetupInformation.CachePath,
                                ShadowCopyFiles = "true",
                                ShadowCopyDirectories = AppDomain.CurrentDomain.SetupInformation.ShadowCopyDirectories
                            };

            RatingDomain = AppDomain.CreateDomain(
                "RatingEngine", null, setup);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(p => p.FullName.StartsWith("System"))
                )
                RatingDomain.Load(assembly.FullName);

            RatingDomain.Load(new AssemblyName("More.Engine"));
        }

        #endregion Methods
    }
}