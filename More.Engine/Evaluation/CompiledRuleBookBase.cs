using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using More.Application.BaseModel;
using More.Engine.BaseModel;
using More.Engine.Model;

namespace More.Engine.Evaluation
{
    public class CompiledRuleBookBase : MarshalByRefObject, IRuleBook
    {
        /// <summary>
        /// The Current Loop Context Item Identifier Stack
        /// </summary>
        public Stack<string> ContextIdStack { get; set; }

        /// <summary>
        /// The Current Loop Context Item Identifier Stack
        /// </summary>
        public string CurrentContextId
        {
            get
            {
                if (ContextIdStack.Count < 1) return null;
                return ContextIdStack.Peek();
            }
        }

        /// <summary>
        /// The Current Loop Context we are on
        /// </summary>
        public string CurrentLoopContext
        {
            get
            {
                if (LoopContextStack.Count < 1) return null;
                return LoopContextStack.Peek();
            }
        }

        /// <summary>
        /// The current loop item columns we are on
        /// </summary>
        public IDictionary<string, object> CurrentLoopItem
        {
            get
            {
                if (CurrentLoopItemStack.Count < 1) return null;
                return CurrentLoopItemStack.Peek();
            }
        }

        /// <summary>
        /// The current loop item we are on stack
        /// </summary>
        public Stack<IDictionary<string, object>> CurrentLoopItemStack { get; set; }

        /// <summary>
        /// The current result columns item we are on
        /// Ex. PowerUnit -> Iso_LiabilityCoverage
        /// </summary>
        public IDictionary<string, object> CurrentResultItem
        {
            get
            {
                if (ResultItemsStack.Count < 1)
                    return Result.Results;

                return ResultItemsStack.Peek();
            }
        }

        public CompiledRuleBookFactoryBase Factory
        {
            get;
            set;
        }

        public bool IncludeTrace { get; set; }

        public IDictionary<string, object> Inputs { get; set; }

        /// <summary>
        /// The Current Loop Context we are on
        /// </summary>
        public Stack<string> LoopContextStack { get; set; }

        /// <summary>
        /// The final result of the rater
        /// </summary>
        public RuleBookResult Result { get; set; }

        /// <summary>
        /// The current result item list we are on
        /// </summary>
        public Stack<IDictionary<string, object>> ResultItemsStack { get; set; }

        public IRatingEngineTableRepository TableRepository
        {
            get;
            set;
        }

        /// <summary>
        /// The current loop item we are on stack
        /// </summary>
        public Stack<RuleBookTraceInformation> TraceInfoStack { get; set; }

        protected RuleBookTraceInformation CurrentTraceInfo
        {
            get
            {
                if (TraceInfoStack.Count < 1) return null;
                return TraceInfoStack.Peek();
            }
        }

        public Dictionary<string, object> Execute(string rulebookName, params object[] args)
        {
            var rb = Factory.GetRuleBook(rulebookName);

            //rb.Inputs = this.Result.Results;
            rb.IncludeTrace = this.IncludeTrace;
            //            rb.TraceInfoStack
            var res = rb.Execute(Result.Results);
            return res.Results;
        }

        public RuleBookResult Execute(Dictionary<string, object> inputs)
        {
            //ResultItemsStack.Push(inputs);

            try
            {
                Initialize();
                //Result.Results = inputs;
                Inputs = inputs;
                return InternalExecute(inputs);
            }
            catch (Exception ex)
            {
                Result.Errors.Add(new RuleBookError(null, ex.Message) { Exception = ex });
            }
            return Result;
        }

        public RuleBookResult Execute(Dictionary<string, object> inputs, int numberTimes)
        {
            RuleBookResult result = null;
            for (int i = 0; i < numberTimes; i++)
            {
                result = Execute(inputs);
            }
            return result;
        }

        public virtual IEnumerable<InputRequirement> GetInputRequirements()
        {
            return null;
        }

        public void Initialize()
        {
            Result = new RuleBookResult();
            TraceInfoStack = new Stack<RuleBookTraceInformation>();
            LoopContextStack = new Stack<string>();
            ContextIdStack = new Stack<string>();
            CurrentLoopItemStack = new Stack<IDictionary<string, object>>();
            ResultItemsStack = new Stack<IDictionary<string, object>>();
            var rootTrace = new RuleBookTraceInformation(string.Empty, "", "Root", true);
            Result.TraceInformation.Add(rootTrace);
            TraceInfoStack.Push(rootTrace);
        }

        protected void AddError(string ruleId, string message, string description)
        {
            Result.Errors.Add(new RuleBookError("RUNTIME", message, ruleId, description));
        }

        protected void AddTrace(int id, string ruleId, string expression, string description, object value, bool push = false)
        {
            //if (!IncludeTrace) return;
            var info = new RuleBookTraceInformation(ruleId, expression, description, value)
                           {
                               Id = id,
                               LoopContext = CurrentLoopContext,
                               ContextId = CurrentContextId
                           };
            if (push)
            {
                CurrentTraceInfo.TraceInformation.Add(info);
                TraceInfoStack.Push(info);
            }
            else
            {
                CurrentTraceInfo.TraceInformation.Add(info);
            }
        }

        protected object AddValue(string name, object value)
        {
            if (CurrentResultItem.ContainsKey(name))
                CurrentResultItem.Remove(name);

            if (CurrentLoopItem != null && CurrentLoopItem.ContainsKey(name))
                CurrentLoopItem.Remove(name);

            CurrentResultItem.Add(name, value);
            if (CurrentLoopItem != null)
                CurrentLoopItem.Add(name, value);
            return value;
        }

        protected void ConditionEntered(string name)
        {
            AddTrace(0, "", "", name, true, true);
            // Push a new result item onto the stack
            var dictionary = new Dictionary<string, object>();
            CurrentResultItem.Add(name, dictionary);
            ResultItemsStack.Push(dictionary);
        }

        protected void ConditionExited(string name)
        {
            TraceInfoStack.Pop();
            ResultItemsStack.Pop();
        }

        protected object GetInput(params string[] names)
        {
            object item = null;

            foreach (var name in names)
            {
                item = GetInput(name, item as IDictionary<string, object>);
            }
            return item;
        }

        protected object GetInput(string name, IDictionary<string, object> context = null)
        {
            if (context != null && context.ContainsKey(name))
            {
                return context[name];
            }
            if (CurrentLoopItem != null && CurrentLoopItem.ContainsKey(name))
            {
                return CurrentLoopItem[name];
            }
            if (Inputs.ContainsKey(name))
            {
                return Inputs[name];
            }
            var resultItemDictionary = ResultItemsStack.FirstOrDefault(p => p.ContainsKey(name));
            if (resultItemDictionary != null)
            {
                if (!resultItemDictionary.ContainsKey(name))
                {
                    throw new Exception(string.Format("The key {0} was not found", name));
                }
                return resultItemDictionary[name];
            }
            if (Result.Results.ContainsKey(name))
            {
                return Result.Results[name];
            }
            AddError(null, string.Format("The variable {0} was not found.", name), string.Format("The variable {0} was not found.", name));
            return null;
        }

        protected virtual RuleBookResult InternalExecute(Dictionary<string, object> inputs)
        {
            return null;
        }

        protected object Lookup(string schema, string name, string column, params object[] args)
        {
            var lookupTable = TableRepository.GetLookupTable(column, name, Factory.EffectiveDate);
            return lookupTable.Lookup(column, args);

            //var rows = factorTableRows.Where(p => p.IsValid(this)).ToArray();
            //var row = rows.FirstOrDefault(p => p.IsMatch(args));
            //if (row == null) return null;
            //return row.Execute(column);
        }

        protected void LoopEntered(string name, string context)
        {
            LoopContextStack.Push(context);
        }

        protected void LoopExited(string name, string context)
        {
            LoopContextStack.Pop();
            TraceInfoStack.Pop();
        }

        protected void LoopItemEntered(int index, string name, string context, IDictionary<string, object> item)
        {
            CurrentLoopItemStack.Push(item);
            // Push Item Stack
            var itemResult = new Dictionary<string, object>();
            CurrentResultItem.Add(index.ToString(), itemResult);
            ResultItemsStack.Push(itemResult);

            ContextIdStack.Push(index.ToString());
            AddTrace(0, "", context + " - " + index, context + " - " + index, true, true);
        }

        protected void LoopItemExited(int index, string name, string context)
        {
            ResultItemsStack.Pop();
            CurrentLoopItemStack.Pop();
            ContextIdStack.Pop();
            TraceInfoStack.Pop();
        }

        protected object StatementExecuted(string id, string name, string description, string expression, Func<object> action)
        {
            try
            {
                var value = action();
                AddTrace(1, id, expression, name, value);
                AddValue(name, value);
                return value;
            }
            catch (Exception ex)
            {
                AddError(id, string.Format("Error in rule {0}:{1}.\r\n{2} ", name, expression, ex.Message), expression);
            }
            return null;
        }

        #region Logic Operations

        public bool ExactMatch(object a, object b)
        {
            if (a is string && b is string)
            {
                return ((string)a).ToUpper().Equals(((string)b).ToUpper());
            }
            else if (a is string)
            {
                return ((string)a).ToUpper().Equals(b.ToString().ToUpper());
            }
            else if (b is string)
            {
                return ((string)b).ToUpper().Equals(a.ToString().ToUpper());
            }
            else if (a is double || b is double)
            {
                return Convert.ToDouble(a) == Convert.ToDouble(b);
            }
            return a.Equals(b);
        }

        public double GetNumber(object a)
        {
            if (a is int)
            {
                return (int)a;
            }
            return Convert.ToDouble(a);
        }

        public bool GreaterThan(object left, object right)
        {
            return GetNumber(left) > GetNumber(right);
        }

        public bool GreaterThanOrEqual(object left, object right)
        {
            return GetNumber(left) >= GetNumber(right);
        }

        public bool LessThan(object left, object right)
        {
            return GetNumber(left) < GetNumber(right);
        }

        public bool LessThanOrEqual(object left, object right)
        {
            return GetNumber(left) <= GetNumber(right);
        }

        #endregion Logic Operations

        #region Math Operations

        public double Age(object year)
        {
            return DateTime.Now.Year - Convert.ToDouble(year);
        }

        public double Age(double year)
        {
            return DateTime.Now.Year - year;
        }

        public double Avg(IEnumerable<object> objs)
        {
            if (objs == null || objs.Count() < 1) return 0;

            return GetNumbers(objs).Average();
        }

        public double Avg(params object[] objs)
        {
            if (objs == null || objs.Length < 1) return 0;
            return GetNumbers(objs).Average();
        }

        public double Count(IList obj)
        {
            return obj.Count;
        }

        public double Count(IEnumerable<object> objs)
        {
            return objs.Count();
        }

        public double Count(object objs)
        {
            if (objs is IList)
            {
                return ((IList)objs).Count;
            }
            return 0;
        }

        public string DateAdd(object dateTime,
                              object interval,
                              object amount)
        {
            var dt = DateTime.Parse(ToDate(dateTime));

            if (!(interval is string) &&
                !(amount is long) &&
                !(amount is int) &&
                !(amount is double))
            {
                throw new InvalidCastException();
            }

            switch (interval.ToString())
            {
                case "d":
                    dt = dt.AddDays((double)amount);
                    break;

                case "h":
                    dt = dt.AddHours((double)amount);
                    break;

                case "n":
                    dt = dt.AddMinutes((double)amount);
                    break;

                case "m":
                    dt = dt.AddMonths((int)amount);
                    break;

                case "s":
                    dt = dt.AddSeconds((double)amount);
                    break;

                case "y":
                    dt = dt.AddYears((int)amount);
                    break;
            }

            return dt.ToString();
        }

        public long DateDiff(object dateTime1, object dateTime2)
        {
            DateTime dt1, dt2;

            if (!DateTime.TryParse(dateTime1.ToString(), out dt1) &
                !DateTime.TryParse(dateTime2.ToString(), out dt2))
            {
                throw new InvalidCastException();
            }

            return Math.Abs(dt2.Subtract(dt1).Ticks);
        }

        public int Day(object dateTime)
        {
            dateTime = ToDate(dateTime);
            return ((DateTime)dateTime).Day;
        }

        public IEnumerable<double> GetNumbers(IEnumerable<object> objects)
        {
            if (objects == null || objects.Count() < 1) return new double[] { 0 };
            var objs = objects.ToArray();
            var ints = objs.OfType<int>().Select(Convert.ToDouble);
            var floats = objs.OfType<float>().Select(Convert.ToDouble);
            var decimals = objs.OfType<decimal>().Select(Convert.ToDouble);
            var doubles = objs.OfType<double>();
            var longs = objs.OfType<long>().Select(Convert.ToDouble);
            return ints.Concat(floats).Concat(decimals).Concat(doubles).Concat(longs);
        }

        public double Low(IEnumerable<object> objs)
        {
            if (objs == null || objs.Count() < 1) return 0;
            return objs.OfType<int>().Select(Convert.ToDouble).Union(
                objs.OfType<float>().Select(Convert.ToDouble)).Union(
                    objs.OfType<double>()
                ).Min();
        }

        public double Max(IEnumerable<object> objs)
        {
            if (objs == null || objs.Count() < 1) return 0;
            return objs.OfType<int>().Select(Convert.ToDouble).Union(
                objs.OfType<float>().Select(Convert.ToDouble)).Union(
                    objs.OfType<double>()
                ).Min();
        }

        public double Max(object objs, string variable)
        {
            return Max((List<Dictionary<string, object>>)objs, variable);
        }

        public double Max(List<Dictionary<string, object>> objs, string variable)
        {
            if (objs == null || !objs.Any()) return 0;

            return objs.Max(p => Convert.ToDouble(p[variable]));
        }

        public double Min(object objs, string variable)
        {
            return Min((List<Dictionary<string, object>>)objs, variable);
        }

        public double Min(List<Dictionary<string, object>> objs, string variable)
        {
            if (objs == null || !objs.Any()) return 0;
            return objs.Min(p => Convert.ToDouble(p[variable]));
        }

        public int Month(object dateTime)
        {
            dateTime = ToDate(dateTime);
            return ((DateTime)dateTime).Month;
        }

        public double Round(object number, int digits)
        {
            return Math.Round(Convert.ToDouble(number), digits);
        }

        public double Round(double number, int digits)
        {
            return Math.Round(number, digits);
        }

        public double Sum(object objs, string variable)
        {
            return Sum((List<Dictionary<string, object>>)objs, variable);
        }

        public double Sum(List<Dictionary<string, object>> objs, string variable)
        {
            return objs.Select(p => Convert.ToDouble(GetInput(variable, p))).Sum();
        }

        public double Sum(IEnumerable<object> objs)
        {
            if (objs == null || objs.Count() < 1) return 0;

            return GetNumbers(objs).Sum();
        }

        public string ToDate(object dateTime)
        {
            var dt = DateTime.MinValue;

            //String formatted date value
            if (dateTime is string)
            {
                dt = DateTime.Parse(dateTime.ToString());
            }

            //Ticks
            if (dateTime is long)
            {
                dt = new DateTime((long)dateTime);
            }

            if (dateTime is DateTime)
            {
                dt = (DateTime)dateTime;
            }

            return dt.ToString(CultureInfo.InvariantCulture);
        }

        public double TotalDays(object timeSpan)
        {
            timeSpan = ToTimeSpan(timeSpan);
            var ts = new TimeSpan((long)timeSpan);
            return Math.Floor(ts.TotalDays);
        }

        public double TotalHours(object timeSpan)
        {
            timeSpan = ToTimeSpan(timeSpan);
            var ts = new TimeSpan((long)timeSpan);
            return Math.Floor(ts.TotalHours);
        }

        public double TotalMinutes(object timeSpan)
        {
            timeSpan = ToTimeSpan(timeSpan);
            var ts = new TimeSpan((long)timeSpan);
            return Math.Floor(ts.TotalMinutes);
        }

        public double TotalMonths(object timeSpan)
        {
            timeSpan = ToTimeSpan(timeSpan);
            var ts = new TimeSpan((long)timeSpan);
            return Math.Floor(ts.TotalDays / (365 / 12));
        }

        public double TotalYears(object timeSpan)
        {
            timeSpan = ToTimeSpan(timeSpan);
            var ts = new TimeSpan((long)timeSpan);
            return Math.Floor(ts.TotalDays / 365);
        }

        public long ToTimeSpan(object timeSpan)
        {
            if (timeSpan is DateTime)
            {
                return ((DateTime)timeSpan).Ticks;
            }

            if (timeSpan is TimeSpan)
            {
                return ((TimeSpan)timeSpan).Ticks;
            }

            if (timeSpan is long)
            {
                return (long)timeSpan;
            }

            var dt = ToDate(timeSpan);
            return DateTime.Parse(dt).Ticks;
        }

        public long Year(object dateTime)
        {
            if (!(dateTime is DateTime))
            {
                dateTime = ToDate(dateTime);
            }

            return ((DateTime)dateTime).Year;
        }

        #endregion Math Operations
    }

    public class FactorTableRow
    {
        private readonly Dictionary<string, object> _columns = new Dictionary<string, object>();
        private readonly List<Func<object, bool>> _keyMatchers = new List<Func<object, bool>>();

        public Dictionary<string, object> Columns
        {
            get { return _columns; }
        }

        public DateTime EffectiveDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public List<Func<object, bool>> KeyMatchers
        {
            get { return _keyMatchers; }
        }

        public FactorTableRow()
        {
        }

        public object Execute(string column)
        {
            return Columns.FirstOrDefault(p => String.Equals(p.Key, column, StringComparison.CurrentCultureIgnoreCase)).Value;
        }

        public bool IsMatch(params object[] args)
        {
            for (int index = 0; index < args.Length; index++)
            {
                var arg = args[index];
                if (!KeyMatchers[index](arg))
                    return false;
            }

            return true;
        }

        public bool IsValid(CompiledRuleBookBase rb)
        {
            // Check for effectivedate
            return true;
        }

        //public bool ExactMatch(object a, object b)
        //{
        //    if (a is string && b is string)
        //    {
        //        return ((string)a).ToUpper().Equals(((string)b).ToUpper());
        //    }
        //    else if (a is string)
        //    {
        //        return ((string)a).ToUpper().Equals(b.ToString().ToUpper());
        //    }
        //    else if (b is string)
        //    {
        //        return ((string)b).ToUpper().Equals(a.ToString().ToUpper());
        //    }
        //    else if (a is double || b is double)
        //    {
        //        return Convert.ToDouble(a) == Convert.ToDouble(b);
        //    }
        //    return a.Equals(b);
        //}
        //public bool LessThanOrEqual(object left, object right)
        //{
        //    return GetNumber(left) <= GetNumber(right);
        //}
        //public bool LessThan(object left, object right)
        //{
        //    return GetNumber(left) < GetNumber(right);
        //}
        //public bool GreaterThanOrEqual(object left, object right)
        //{
        //    return GetNumber(left) >= GetNumber(right);
        //}
        //public bool GreaterThan(object left, object right)
        //{
        //    return GetNumber(left) > GetNumber(right);
        //}
        //public double GetNumber(object a)
        //{
        //    if (a is int)
        //    {
        //        return (int)a;
        //    }
        //    return Convert.ToDouble(a);
        //}
    }
}