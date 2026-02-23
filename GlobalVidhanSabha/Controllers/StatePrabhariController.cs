using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VishanSabha.ActionFilter;

namespace GlobalVidhanSabha.Controllers
{
    [Authorize(Roles = "StatePrabhari")]
 
    public class StatePrabhariController : Controller
    {
        // GET: Prabhari

        public ActionResult VidhanSabhaRagiter()
        {
            var stateId = Session["StateId"];
            Console.WriteLine("my id", stateId);

            if (stateId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // ✅ ADD THIS LINE
            ViewBag.StateId = stateId;
            return View();
        }
        public ActionResult AddSamithiMember()
        {
            return View();
        }
        public ActionResult PrabhariDashboard()
        {
            return View();

        }
              public ActionResult ShowDistrictCount()
        {
            int stateId = Convert.ToInt32(Session["StateId"]);

            ViewBag.StateId = stateId;

            return View();
        }

    }
}