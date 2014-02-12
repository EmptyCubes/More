using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using More.Application;
using More.Application.BaseModel;
using More.Web.Designer.Models;

namespace More.Web.Designer.Controllers
{
    public class BaseController : Controller
    {
        // Protected Methods (1) 

        private DateVersion[] _effectiveDates = null;

        private DateTime? _missionControlDate;

        public int CurrentRuleBookId
        {
            get { return Convert.ToInt32(Session["CurrentRateBookId"]); }
            set { Session["CurrentRateBookId"] = value; }
        }

        public bool IsReadOnlyMode
        {
            get
            {
                return GetMostRecentEffectiveDate() != MissionControlDate;
            }
        }

        public virtual DateTime MissionControlDate
        {
            get
            {
                if (_missionControlDate != null)
                {
                    return _missionControlDate.Value;
                }
                var cookie = Request.Cookies["MissionControlDate"];
                if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                {
                    return (DateTime)(_missionControlDate = GetMostRecentEffectiveDate());
                }
                return (DateTime)(_missionControlDate = Convert.ToDateTime(cookie.Value));
            }
            set
            {
                _missionControlDate = value;
                Response.Cookies.Add(new HttpCookie("MissionControlDate", value.ToString()) { Path = "/", Expires = DateTime.Now.AddDays(2) });
                ViewBag.MissionControlDate = value;
            }
        }

        public string StepInto
        {
            get { return Session["StepInto"] == null ? null : Session["StepInto"].ToString(); }
            set { Session["StepInto"] = value; }
        }

        public IRulesEngineRepository Strategy
        {
            get
            {
                return MoreApplicationFactory.GetEngineRepository(MissionControlDate, true);
            }
        }

        public ILookupTablesRepository TableStrategy
        {
            // Prob not the best approach but works for now
            get { return MoreApplicationFactory.GetLookupRepository(MissionControlDate, true); }
        }

        protected virtual IEnumerable<DateVersion> EffectiveDates
        {
            get
            {
                return _effectiveDates ?? (_effectiveDates = MoreApplicationFactory.GetEngineRepository(DateTime.Now, true).GetRatingAssemblies().Select(p => new DateVersion(p.EffectiveDate, 0)).ToArray());
            }
        }

        public ActionResult SetMissionControlDate(DateTime date)
        {
            MissionControlDate = date;
            ViewBag.MissionControlDate = MissionControlDate;
            return RedirectToAction("Index");
        }

        protected virtual DateTime GetMostRecentEffectiveDate()
        {
            try
            {
                return
                    EffectiveDates.Where(p => p.EffectiveDate > DateTime.Now).OrderBy(p => p.EffectiveDate).
                        First().EffectiveDate;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ViewBag.StepInto = StepInto;
            ViewBag.BaseKeys = Strategy.GetRuleBooks();
            ViewBag.CurrentBaseKey = CurrentRuleBookId;
            ViewBag.MissionControlDate = MissionControlDate;
            ViewBag.MissionControlDates = EffectiveDates;
        }
    }
}