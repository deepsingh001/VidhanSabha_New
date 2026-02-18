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
    }
}