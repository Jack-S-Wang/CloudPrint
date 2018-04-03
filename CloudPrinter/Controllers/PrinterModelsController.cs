using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudPrinter.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Web;

namespace CloudPrinter.Controllers
{
    public class PrinterModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        int length = 20;
       
        // GET: PrinterModels/5
        public ActionResult Index(string userAccount)
        {
            Response.CacheControl = "no-cache";
            if (userAccount == null || userAccount == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account");
            }
            var result = Request.Cookies.Get("LoginAccount").Value;
            if (result == "" || result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Account is Expire");
            }
            ViewBag.loginAccount = result;
            ViewBag.superAccount = superAdmin.AdminInfo.ADMINACCOUNT;
            ViewBag.userAccount = userAccount;
            HttpCookie cookie = new HttpCookie("ToCount");
            cookie["count"] = "0";
            if (Request.Cookies.Get("ToCount") != null)
            {
                Response.Cookies.Set(cookie);
            }
            else
            {
                Response.Cookies.Add(cookie);
            }
            Response.Cookies["ToCount"].Expires = DateTime.Now.AddHours(2);
            cookie.Expires = DateTime.Now.AddHours(2);
            Response.Cookies.Add(cookie);
            List<PrinterModels> li = new List<PrinterModels>();
            li = db.PrinterModels.OrderBy(c => c.printerNumber).Skip(co * length).Take(length).ToList();
            return View(li);
        }

        // GET: PrinterModels/Details/5
        public ActionResult Details(string printerNumber)
        {

            if (printerNumber == null || printerNumber == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Device");
            }
            var result = Request.Cookies.Get("LoginAccount").Value;
            if (result == "" || result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Account is Expire");
            }
            ViewBag.loginAccount = result;
            PrinterModels printerModels = db.PrinterModels.Find(printerNumber);
            if (printerModels == null)
            {
                return HttpNotFound();
            }
            return View(printerModels);
        }

        // GET: PrinterModels/Create/5
        public ActionResult Create(string userAccount)
        {
            if (userAccount == null || userAccount == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account");
            }
            var result = Request.Cookies.Get("LoginAccount").Value;
            if (result == "" || result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Account is Expire");
            }
            ViewBag.loginAccount = result;
            ViewBag.userAccount = userAccount;
            return View();
        }

        // POST: PrinterModels/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userAccount,printerNumber,printerName")] PrinterModels printerModels, string maxPrinter)
        {
            if (maxPrinter != printerModels.printerNumber)
            {
                long max = Convert.ToInt64(maxPrinter, 16);
                long min = Convert.ToInt64(printerModels.printerNumber, 16);
                var umodel = db.UserModels.Find(printerModels.userAccount);
                for (long i = 0; i < max - min; i++)
                {
                    long number = min + i;
                    PrinterModels printer = new PrinterModels()
                    {
                        printerNumber = string.Format("{0:X16}", number),
                        printerName = printerModels.printerName,
                        cState = "ready",
                        stateMessage = "就绪",
                        userName = umodel.Name,
                        userAccount = printerModels.userAccount,
                        registerTime = DateTime.Now,
                        mState = false
                    };
                    var print = db.PrinterModels.Find(printer.printerNumber);
                    if (printer == null)
                    {
                        db.PrinterModels.Add(printer);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { userAccount = printerModels.userAccount });
            }
            else
            {
                ModelState.Remove("cState");
                printerModels.cState = "ready";
                printerModels.stateMessage = "就绪";
                var umodel = db.UserModels.Find(printerModels.userAccount);
                printerModels.userName = umodel.Name;
                printerModels.registerTime = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.PrinterModels.Add(printerModels);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { userAccount = printerModels.userAccount });
                }
            }

            return View(printerModels);
        }

        // GET: PrinterModels/Edit/5
        public ActionResult Edit(string printerNumber)
        {
            if (printerNumber == null || printerNumber == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Device");
            }
            var result = Request.Cookies.Get("LoginAccount").Value;
            if (result == "" || result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Account is Expire");
            }
            ViewBag.loginAccount = result;
            ViewBag.superAccount = superAdmin.AdminInfo.ADMINACCOUNT;
            PrinterModels printerModels = db.PrinterModels.Find(printerNumber);
            if (printerModels == null)
            {
                return HttpNotFound();
            }
            return View(printerModels);
        }

        // POST: PrinterModels/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "printerNumber,printerName,cState")] PrinterModels printerModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(printerModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { userAccount = printerModels.userAccount });
            }
            return View(printerModels);
        }

        // GET: PrinterModels/Delete/5
        public ActionResult Delete(string printerNumber)
        {
            if (printerNumber == null || printerNumber == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Device");
            }
            var result = Request.Cookies.Get("LoginAccount").Value;
            if (result == "" || result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Account is Expire");
            }
            ViewBag.loginAccount = result;
            PrinterModels printerModels = db.PrinterModels.Find(printerNumber);
            if (printerModels == null)
            {
                return HttpNotFound();
            }
            return View(printerModels);
        }

        // POST: PrinterModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string printerNumber)
        {
            PrinterModels printerModels = db.PrinterModels.Find(printerNumber);
            var userAccount = printerModels.userAccount;
            db.PrinterModels.Remove(printerModels);
            db.SaveChanges();
            return RedirectToAction("Index", new { userAccount = userAccount });
        }
        int co = 0;
        // GET : PrinterModels/PerUserList/5
        [HttpPost]
        public ActionResult PerUserList(string userAccount)
        {
            Response.CacheControl = "no-cache";
            if (userAccount == null || userAccount == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account");
            }
            var result = Request.Cookies.Get("LoginAccount").Value;
            if (result == "" || result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Account is Expire");
            }
            ViewBag.loginAccount = result;
            ViewBag.superAccount = superAdmin.AdminInfo.ADMINACCOUNT;
            ViewBag.userAccount = userAccount;

            List<PrinterModels> li = new List<PrinterModels>();
            co = Int32.Parse(Request.Cookies.Get("ToCount").Values["count"]);
            try
            {
                li = db.PrinterModels.OrderBy(c=>c.printerNumber).Skip(co * length).Take(length).ToList();
            }catch(Exception ex)
            {
                string s = string.Format("{0}", ex);
            } 
            return PartialView("PerUserList", li);
        }
        [HttpPost]
        public ActionResult upToList(string userAccount)
        {
            HttpCookie cookie = Request.Cookies["ToCount"];
            if (cookie != null)
            {
                string s = cookie.Values["count"];
                if (s != "0")
                {
                    co = Int32.Parse(s) - 1;
                }
                    cookie.Values["count"] = co.ToString();
                    //Response.Cookies.Set(cookie);
                    //Response.Cookies["ToCount"].Expires = DateTime.Now.AddHours(2);
                    cookie.Expires = DateTime.Now.AddHours(2);
                    Response.Cookies.Set(cookie);
            }
            return PerUserList(userAccount);
        }
        [HttpPost]
        public ActionResult nextToList(string userAccount)
        {
            try
            {
                HttpCookie cookie = Request.Cookies["ToCount"];
                if (cookie != null)
                {
                    string s = cookie.Values["count"];
                    int con = db.PrinterModels.ToList().Count % 20 > 0 ? (db.PrinterModels.ToList().Count / 20) + 1 : db.PrinterModels.ToList().Count / 20;
                    if (Int32.Parse(s) <con )
                    {
                        co = Int32.Parse(s) + 1;
                    }else
                    {
                        co = Int32.Parse(s);
                    }
                    cookie.Values["count"] = co.ToString();
                    //Response.Cookies.Set(cookie);
                    //Response.Cookies["ToCount"].Expires = DateTime.Now.AddHours(2);
                    cookie.Expires = DateTime.Now.AddHours(2);
                    Response.Cookies.Set(cookie);
                }
            }
            catch(Exception ex)
            {
                string s = string.Format("{0}", ex);
            }
            return PerUserList(userAccount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
