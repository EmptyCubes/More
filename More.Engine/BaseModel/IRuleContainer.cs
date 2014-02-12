using More.Engine.Model;

namespace More.Engine.BaseModel
{
    /// <summary>
    /// A class which contains a set of RuleBookRules
    /// </summary>
    public interface IRuleContainer
    {
        RulesCollection Children { get; set; }
    }
}