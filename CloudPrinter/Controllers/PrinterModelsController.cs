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

        // GET: PrinterModels
        public ActionResult Index()
        {
            return View(db.PrinterModels.ToList());
        }

        // GET: PrinterModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrinterModels printerModels = db.PrinterModels.Find(id);
            if (printerModels == null)
            {
                return HttpNotFound();
            }
            return View(printerModels);
        }

        // GET: PrinterModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrinterModels/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrinterModelsId,userId,printerName,mState,cState,stateMessage,userName")] PrinterModels printerModels)
        {
            printerModels.registerTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.PrinterModels.Add(printerModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(printerModels);
        }

        // GET: PrinterModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrinterModels printerModels = db.PrinterModels.Find(id);
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
        public ActionResult Edit([Bind(Include = "PrinterModelsId,userId,printerName,mState,cState,stateMessage,userName")] PrinterModels printerModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(printerModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(printerModels);
        }

        // GET: PrinterModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrinterModels printerModels = db.PrinterModels.Find(id);
            if (printerModels == null)
            {
                return HttpNotFound();
            }
            return View(printerModels);
        }

        // POST: PrinterModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrinterModels printerModels = db.PrinterModels.Find(id);
            db.PrinterModels.Remove(printerModels);
            db.SaveChanges();
            return RedirectToAction("Index");
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
