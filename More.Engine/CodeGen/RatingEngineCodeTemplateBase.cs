using System;
using System.Collections.Generic;
using System.Linq;
using More.Application.BaseModel;
using More.Engine.CodeGen.Templates.Model;
using More.Engine.Model;

namespace More.Engine.CodeGen
{
    public class RatingEngineCodeTemplateBase<TModel> : T4TemplateCodeBase
    {
        public RulesEngineCodeContext Context { get; set; }

        public TModel Model { get; set; }

        public string Name { get { return Context.LibraryName; } }

        public void EnterContext()
        {
            Context.DeclaredVariables.PushContext();
        }

        public void LeaveContext()
        {
            Context.DeclaredVariables.PopContext();
        }

        public IEnumerable<LookupTableRow> SortByLookupType(IEnumerable<LookupTableRow> rows, FactorTableColumnDefinition sortColumn, int sortColumnIndex)
        {
            switch (sortColumn.LookupType)
            {
                case LookupType.GreaterThanOrEqual:
                case LookupType.GreaterThan:
                    return rows.OrderByDescending(p => Decimal.Parse((string)p.KeyValues[sortColumnIndex].LowValue));

                case LookupType.LessThan:
                case LookupType.LessThanOrEqual:
                    return rows.OrderBy(p => Decimal.Parse(p.KeyValues[sortColumnIndex].LowValue));
            }
            return rows;
        }

        public void WriteTemplate<T, T2>(T2 model) where T : RatingEngineCodeTemplateBase<T2>, new()
        {
            var t = new T()
                        {
                            Context = Context,
                            Model = model
                        };

            Write(t.TransformText());
        }

        public void WriteVariableDecleration(string name)
        {
            Write("{0} {1}", Context.VarWhenNeeded(name), name);
            Context.DeclaredVariables.Add(name);
        }

        protected string GetValueString(object value)
        {
            double o;
            if (double.TryParse(value.ToString(), out o))
                return o.ToString();
            if (value is string)
            {
                return "\"" + value + "\".ToLower()";
            }
            return value.ToString();
        }
    }
}