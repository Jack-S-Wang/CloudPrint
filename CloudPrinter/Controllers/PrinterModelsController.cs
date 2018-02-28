using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudPrinter.Models;

namespace CloudPrinter.Controllers
{
    public class PrinterModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: PrinterModels/5
        public ActionResult Index(string userAccount)
        {
            if (userAccount == null || userAccount=="")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account");
            }
            var result = Request.Cookies.Get("LoginAccount").Value;
            if(result=="" || result == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Account is Expire");
            }
            ViewBag.loginAccount = result;
            ViewBag.superAccount = superAdmin.AdminInfo.ADMINACCOUNT;
            ViewBag.userAccount = userAccount;
            return View(db.PrinterModels.ToList());
        }

        // GET: PrinterModels/Details/5
        public ActionResult Details(string printerNumber)
        {
            
            if (printerNumber==null || printerNumber=="")
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
            if (userAccount == null || userAccount=="")
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
        public ActionResult Create([Bind(Include = "userAccount,printerNumber,printerName")] PrinterModels printerModels)
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
                return RedirectToAction("Index",new { userAccount=printerModels.userAccount});
            }

            return View(printerModels);
        }

        // GET: PrinterModels/Edit/5
        public ActionResult Edit(string printerNumber)
        {
            if (printerNumber == null || printerNumber=="")
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
                return RedirectToAction("Index",new { userAccount=printerModels.userAccount});
            }
            return View(printerModels);
        }

        // GET: PrinterModels/Delete/5
        public ActionResult Delete(string printerNumber)
        {
            if (printerNumber == null || printerNumber=="")
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
            return RedirectToAction("Index",new { userAccount=userAccount});
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
