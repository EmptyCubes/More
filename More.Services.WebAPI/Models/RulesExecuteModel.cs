using System;
using System.Collections.Generic;
using System.Linq;

namespace More.Services.WebAPI.Models
{

    public class RulesExecuteModel
    {
        public DateTime EffectiveDate { get; set; }
        public string RuleBook { get; set; }
        public Dictionary<string, string> Inputs { get; set; }
        public string Data { get; set; }
        public string[] Items { get; set; }
    }
}
