using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace More.Web.Designer.Helpers
{
    public static class EnumHelpers
    {

        public static IEnumerable<SelectListItem> GetEnumSelectList<T>()
        {
            return (Enum.GetValues(typeof(T)).Cast<T>().Select(
                enu => new SelectListItem() { Text = enu.ToString(), Value = enu.ToString() })).ToList();
        }
    }
}