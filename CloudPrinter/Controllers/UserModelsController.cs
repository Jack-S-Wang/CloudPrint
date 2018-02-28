using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudPrinter.Models;
using System.Collections.Concurrent;

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
        public ActionResult Details(string userAccount)
        {
            if (userAccount == null || userAccount == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account");
            }
            UserModels userModels = db.UserModels.Find(userAccount);
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
        public ActionResult Create([Bind(Include = "userAccount,Name,password")] UserModels userModels, string referrer)
        {
            if (ModelState.IsValid)
            {
                userModels.registerDate = DateTime.Now;
                userModels.RememberMe = false;
                db.UserModels.Add(userModels);
                db.SaveChanges();
                return Redirect(referrer);
            }
            ModelState.AddModelError("", "创建账户失败！");
            return View();
        }

        // GET: UserModels/Edit/5
        public ActionResult Edit(string userAccount)
        {
            if (userAccount == null || userAccount == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account");
            }
            UserModels userModels = db.UserModels.Find(userAccount);
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
        public ActionResult Edit([Bind(Include = "userAccount,Name,password")] UserModels userModels)
        {
            if (ModelState.IsValid)
            {
                var um = db.UserModels.Find(userModels.userAccount);
                if (um != null)
                {
                    um.Name = userModels.Name;
                    um.password = userModels.password;
                    db.Entry(userModels).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }else
                {
                    return HttpNotFound();
                }
            }
            return View(userModels);
        }

        // GET: UserModels/Delete/5
        public ActionResult Delete(string userAccount)
        {
            if (userAccount == null || userAccount == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account");
            }
            UserModels userModels = db.UserModels.Find(userAccount);
            if (userModels == null)
            {
                return HttpNotFound();
            }
            return View(userModels);
        }

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string userAccount)
        {
            UserModels userModels = db.UserModels.Find(userAccount);
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
