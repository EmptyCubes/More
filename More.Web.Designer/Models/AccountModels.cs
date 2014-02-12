using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace More.Web.Designer.Models
{
    
    public class DateVersion
    {
        public DateTime EffectiveDate { get; set; }
        public int NumChanges { get; set; }

        public DateVersion(DateTime effectiveDate, int? numChanges)
        {
            EffectiveDate = effectiveDate;
            NumChanges = numChanges ?? 0;
        }
    }
}
