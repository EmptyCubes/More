using System;
using System.Collections.Generic;
using System.Linq;
using More.Engine.BaseModel;
using More.Engine.CodeGen;
using NCalc;
using NCalc.Domain;

namespace More.Engine.Model
{
    /// <summary>
    /// RatingRule is a model class plus a rule information "work-horse".
    /// Its primary role is to contain as much information as it can about a rule as possible.
    /// Alot of its properties parse the actual expression for additional information about this rule.
    /// </summary>
    public class RuleEngineRule : IRuleContainer
    {
        #region Fields (4)

        public string[] _inputs;
        public string[] _lookups;
        private RulesCollection _children;
        private NCalc.Expression _expression;
        private string _ruleExpression;

        #endregion Fields

        #region Constructors (5)

        /// <summary>
        /// Note: sure if this is correct yet.  Needs to be tested. Unit Tests?
        /// </summary>
        public string[] AccessibleVars
        {
            get
            {
                if (Parent == null)
                {
                    return Outputs;
                }
                return Outputs.Union(Parent.AccessibleVars).ToArray();
            }
        }

        /// <summary>
        /// Is this rule active?
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The lookups invoked in this rule as well as all the childrens. And their childrens. etc
        /// </summary>
        public string[] AllInputs
        {
            get
            {
                if (HasErrors) return new string[] { };
                if (Children.Count > 0)
                {
                    return Inputs.Union(Children.SelectMany(p => p.AllInputs)).Distinct().ToArray();
                }
                return Inputs;
            }
        }

        /// <summary>
        /// The lookups invoked in this rule as well as all the childrens. And their childrens. etc
        /// </summary>
        public string[] AllLookups
        {
            get
            {
                if (HasErrors) return new string[] { };
                if (Children.Count > 0)
                {
                    return Lookups.Union(Children.SelectMany(p => p.AllLookups)).Distinct().ToArray();
                }
                return Lookups;
            }
        }

        /// <summary>
        /// Usually a guid which is an id linking this rule to past versions of this rule
        /// </summary>
        public string ChangeId { get; set; }

        /// <summary>
        /// Self-explanatory
        /// </summary>
        public RulesCollection Children
        {
            get { return _children ?? (_children = new RulesCollection(this)); }
            set { _children = value; }
        }

        public string[] ChildrenExternalInputs
        {
            get
            {
                if (HasErrors) return new string[] { };
                return ChildrenOnlyInputs.Where(p => !Outputs.Contains(p) && !ContextVars.Contains(p)).ToArray();
            }
        }

        /// <summary>
        /// The lookups invoked in this rule as well as all the childrens. And their childrens. etc
        /// </summary>
        public string[] ChildrenOnlyInputs
        {
            get
            {
                if (HasErrors) return new string[] { };
                if (Children.Count > 0)
                {
                    return Inputs.Union(Children.SelectMany(p => p.Inputs)).Distinct().ToArray();
                }
                return Inputs;
            }
        }

        public string[] ContextVars
        {
            get
            {
                if (HasErrors) return new string[] { };
                if (Parent == null)
                {
                    return Outputs;
                }
                return Parent.ContextVars.Union(Outputs).ToArray();
            }
        }

        /// <summary>
        /// The date this becomes effective
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        public Exception ErrorException
        {
            get
            {
                if (this.Expression.ErrorException != null)
                {
                    return Expression.ErrorException;
                }
                var itemWith = Children.FirstOrDefault(p => p.ErrorException != null);
                if (itemWith != null) return itemWith.ErrorException;
                return null;
            }
        }

        public IEnumerable<RuleBookError> Errors
        {
            get
            {
                var errorException = ErrorException;
                if (errorException != null)
                {
                    yield return new RuleBookError("PARSER", errorException.Message, errorException, this.Id, null);
                }
                if (ValidationErrors != null)
                    foreach (var validationError in ValidationErrors)
                    {
                        yield return validationError;
                    }
            }
        }

        public NCalc.Expression Expression
        {
            get
            {
                return _expression ?? (_expression = new Expression(RuleExpression, EvaluateOptions.NoCache));
            }
            private set { _expression = value; }
        }

        public string[] ExternalInputs
        {
            get
            {
                if (HasErrors) return new string[] { };
                return AllInputs.Where(p => !Outputs.Contains(p) && !ContextVars.Contains(p)).ToArray();
            }
        }

        /// <summary>
        /// Don't really think this is being used but it might be useful for something else later on.
        /// </summary>
        public string FullName
        {
            get
            {
                if (Parent == null)
                    return Name;

                return Parent.FullName + "_" + Name;
            }
        }

        public bool HasErrors { get; set; }

        /// <summary>
        /// If this or any children have a forms attachment
        /// </summary>
        public bool HasFormsAttachment
        {
            get { return this.IsFormsAttachment || this.Children.Any(p => p.HasFormsAttachment); }
        }

        /// <summary>
        /// The Id for this rule
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Extracts the variables that this rule uses, Lazy-Loaded
        /// </summary>
        public string[] Inputs
        {
            get
            {
                if (HasErrors) return new string[] { };
                if (_inputs != null)
                {
                    return _inputs;
                }
                var variableExtractor = new NCalcVariableExtractionVisitor();
                ParsedExpression.Accept(variableExtractor);
                return _inputs = variableExtractor.Variables.ToArray();
            }
        }

        public bool IsBase
        {
            get
            {
                return !IsNew && !IsOverride && !IsChange;
            }
        }

        public bool IsChange
        {
            get;
            set;
        }

        /// <summary>
        /// If this has a attach form method in it
        /// </summary>
        public bool IsFormsAttachment
        {
            get { return Lookups.Contains("AttachForm"); }
        }

        public bool IsNew
        {
            get;
            set;
        }

        public bool IsOverride
        {
            get;
            set;
        }

        /// <summary>
        /// Extracts all the methods that this rule uses. Lazy-Loaded
        /// </summary>
        public string[] Lookups
        {
            get
            {
                if (HasErrors) return new string[] { };
                if (_lookups != null)
                {
                    return _lookups;
                }
                var variableExtractor = new NCalcVariableExtractionVisitor();
                ParsedExpression.Accept(variableExtractor);
                return _lookups = variableExtractor.Methods.ToArray();
            }
        }

        /// <summary>
        /// This rule begins a loop of context items such as ( Drivers, PowerUnits, or Trailers )
        /// </summary>
        public string LoopContext { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Note: This is being used but im not sure its the appropriate name for it
        /// </summary>
        public string[] OutOfScopeInputs
        {
            get
            {
                if (HasErrors) return new string[] { };
                return AllInputs.Where(p => !Outputs.Contains(p)).ToArray();
            }
        }

        /// <summary>
        /// All of the Names/Variables that this rule outputs.
        /// Simply ( return this.Name and If Children then return their 'Outputs' as well )
        /// </summary>
        public string[] Outputs
        {
            get
            {
                if (HasErrors) return new string[] { };
                if (Children.Count < 1)
                {
                    return new[] { this.Name };
                }
                return new[] { this.Name }.Union(Children.SelectMany(p => p.Outputs)).ToArray();
            }
        }

        /// <summary>
        /// The rule parent instance
        /// </summary>
        public RuleEngineRule Parent { get; set; }

        /// <summary>
        /// The assigned parent change id
        /// </summary>
        public Guid? ParentChangeId { get; set; }

        public IEnumerable<RuleEngineRule> Parents
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    yield return parent;
                    parent = parent.Parent;
                }
            }
        }

        public LogicalExpression ParsedExpression
        {
            get { return Expression.ParsedExpression; }
        }

        public int RuleBookId { get; set; }

        /// <summary>
        /// The good ol' string expression
        /// </summary>
        public string RuleExpression
        {
            get
            {
                return _ruleExpression;
            }
            set
            {
                _ruleExpression = value;
                _inputs = null;
            }
        }

        /// <summary>
        /// Don't remember why this property is here but its useful for something im sure. Either now, or in the future
        /// </summary>
        public string Tag { get; set; }

        public RuleBookError[] ValidationErrors { get; set; }

        public RuleEngineRule(string id, string name, string expression, RuleEngineRule parent)
        {
            Id = id;
            Name = name;
            RuleExpression = expression;
            Parent = parent;
            Expression = new Expression(expression);
        }

        public RuleEngineRule(string id, string name, string expression)
        {
            Id = id;
            Name = name;
            RuleExpression = expression;
            Expression = new Expression(expression);
        }

        public RuleEngineRule(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public RuleEngineRule(string name, RuleEngineRule parent)
        {
            Name = name;
            Parent = parent;
        }

        public RuleEngineRule()
        {
        }

        #endregion Constructors

        #region Properties (20)
        #endregion Properties

        #region Methods (3)

        // Public Methods (3) 

        /// <summary>
        /// The sort algorithm for rules
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="otherInputs"></param>
        /// <returns></returns>
        public static RuleEngineRule[] SortedRules(IEnumerable<RuleEngineRule> rules, string[] otherInputs = null)
        {
            var ordered = new List<RuleEngineRule>();
            var queue = rules.ToArray();

            var canAdd = new Func<RuleEngineRule, bool>((item) =>
                                                            {
                                                                var inputs = item.OutOfScopeInputs;
                                                                var ruleNames = ordered.Select(p => p.Name).ToArray();
                                                                if (otherInputs != null)
                                                                    ruleNames = ruleNames.Union(otherInputs).ToArray();
                                                                return inputs.All(ruleNames.Contains);
                                                            });

            while (queue.Length > 0)
            {
                // A placed holder for the ones that can be added yet
                var notYetAdded = new List<RuleEngineRule>();

                foreach (var rule in queue)
                {
                    // If we can add it ADD IT!
                    if (canAdd(rule))
                        ordered.Add(rule);
                    else
                        // otherwise add it to a list and wait to add it
                        notYetAdded.Add(rule);
                }

                // If we havn't added any new ones at this point we have
                // one that can't be found and will give us an infinite loop
                if (queue.Length == notYetAdded.Count)
                    throw new Exception(string.Join(", ", notYetAdded.SelectMany(p => p.Inputs)) + " can't been found");

                // Apply the ones that can't be added yet to the queue
                queue = notYetAdded.ToArray();
            }

            return ordered.ToArray();
        }

        public RuleEngineRule Find(string id)
        {
            foreach (var child in Children)
            {
                if (child.Id == id)
                    return child;

                var res = child.Find(id);
                if (res != null)
                    return res;
            }
            return null;
        }

        #endregion Methods
    }
}