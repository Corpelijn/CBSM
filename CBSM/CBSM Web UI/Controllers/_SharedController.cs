using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBSM_Web_UI.Controllers
{
    public class _SharedController : Controller
    {
        public ActionResult Navbar()
        {
            return PartialView("_Navbar");
        }

        public ActionResult _MainHeader()
        {
            return PartialView();
        }

        public ActionResult _MainHeaderEmpty()
        {
            return PartialView();
        }

        public ActionResult _MainSidebar()
        {
            return PartialView();
        }

        public ActionResult _MainFooter()
        {
            return PartialView();
        }

        public ActionResult _ControlSidebar()
        {
            return PartialView();
        }
	}
}