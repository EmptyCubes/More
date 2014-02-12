using More.Engine.CodeGen.Templates.Model;

namespace More.Engine.BaseModel
{
    /// <summary>
    /// Provides the code necessary to compile a rating engine executable
    /// </summary>
    public interface IRulesEngineCodeProvider
    {
        /// <summary>
        /// Returns the code needed to compile a rating engine executable
        /// </summary>
        /// <param name="rulesModel">The information about the rules that is needed for code generation.</param>
        /// <returns></returns>
        string GetCode(RuleEngineCodeModel rulesModel);
    }
}