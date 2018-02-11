using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudPrinter.Controllers
{
    public class UserLoginController : Controller
    {
        // GET: UserLogin
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost,ActionName("LoginIn")]
        public ActionResult LoginIn()
        {
            return RedirectToAction("Index", "PrinterModels");
        }
    }
}