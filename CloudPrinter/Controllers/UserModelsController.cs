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
    public class UserModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserModels
        public ActionResult Index()
        {
            return View(db.UserModels.ToList());
        }

        // GET: UserModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModels userModels = db.UserModels.Find(id);
            if (userModels == null)
            {
                return HttpNotFound();
            }
            return View(userModels);
        }

        // GET: UserModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserModels/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserModelsId,userName,password")] UserModels userModels)
        {
            userModels.registerDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.UserModels.Add(userModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userModels);
        }

        // GET: UserModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModels userModels = db.UserModels.Find(id);
            if (userModels == null)
            {
                return HttpNotFound();
            }
            return View(userModels);
        }

        // POST: UserModels/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserModelsId,userName,password")] UserModels userModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userModels);
        }

        // GET: UserModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModels userModels = db.UserModels.Find(id);
            if (userModels == null)
            {
                return HttpNotFound();
            }
            return View(userModels);
        }

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserModels userModels = db.UserModels.Find(id);
            db.UserModels.Remove(userModels);
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
