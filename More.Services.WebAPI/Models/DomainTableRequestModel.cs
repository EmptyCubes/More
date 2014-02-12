using System;

namespace More.Services.WebAPI.Models
{
    public class DomainTableRequestModel
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public dynamic Filter { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}