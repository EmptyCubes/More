namespace Carolina.Library.Rating.Compiling
{
    /// <summary>
    /// A half-ass modified implementation of the visitor pattern thats not-really the visitor pattern :p
    /// </summary>
    public interface IRatingRuleVisitor
    {
        void VisitRule(RuleEngineRule rule);
    }
}