using CloudPrinter.Models;
using CloudPrinter.superAdmin;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace CloudPrinter.Controllers
{
    public class UserLoginController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserLogin
        public ActionResult Login()
        {
            var cookie = Request.Cookies.Get("name");
            if (cookie != null)
            {
                var um = db.UserModels.Find(cookie.Value);
                if (um != null)
                {
                    return View(um);
                }
            }
            return View();
        }
        [HttpPost, ActionName("LoginIn")]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Client, NoStore = false, Duration = 30)]
        public ActionResult LoginIn([Bind(Include ="userAccount,password")] UserModels userModels,string remember)
        {
            if (ModelState.IsValid)
            {
                bool remb = false;
                if (remember != null)
                {
                    remb = true;
                }
                var um = db.UserModels.Find(userModels.userAccount);
                if (um != null)
                {
                    if (um.password.Equals(userModels.password))
                    {
                        um.RememberMe = remb;
                        db.Entry(um).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        HttpCookie cookie = new HttpCookie("name", userModels.userAccount);
                        Response.Cookies.Set(cookie);
                        Response.Cookies["name"].Expires = DateTime.MaxValue;
                        HttpCookie cookieLogin = new HttpCookie("LoginAccount", userModels.userAccount);
                        if (Request.Cookies.Get("LoginAccount") != null)
                        {
                            Response.Cookies.Set(cookieLogin);
                        }
                        else
                        {
                            Response.Cookies.Add(cookieLogin);
                        }
                        Response.Cookies["LoginAccount"].Expires = DateTime.Now.AddMinutes(1);
                        if (userModels.userAccount.Equals(AdminInfo.ADMINACCOUNT))
                        {
                            return RedirectToAction("Index", "UserModels");
                        }
                        return RedirectToAction("Index", "PrinterModels", new { userAccount = userModels.userAccount });
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"password is wrong!");
                    }
                }else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no find Account！");
                }
            }
            //ModelState.AddModelError("error", "无效登录！");
            return View(userModels);  

            //if (!ModelState.IsValid)
            //{
            //    return View(userModels);
            //}

            //// 这不会计入到为执行帐户锁定而统计的登录失败次数中
            //// 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
            //var result = await SignInManager.PasswordSignInAsync(userModels.userAccount, userModels.password, userModels.RememberMe, shouldLockout: true);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        HttpCookie cookie = new HttpCookie("name", userModels.userAccount);
            //        Response.Cookies.Set(cookie);
            //        Response.Cookies["name"].Expires = DateTime.MaxValue;
            //        if (userModels.userAccount.Equals(AdminInfo.ADMINACCOUNT))
            //        {
            //            return RedirectToAction("Index", "UserModels");
            //        }
            //        return RedirectToAction("Index", "PrinterModels", new { userAccount = userModels.userAccount });
            //    case SignInStatus.LockedOut:
            //        return View("Lockout", "Account");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", "Account", new { ReturnUrl = Request.UrlReferrer, RememberMe = userModels.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "无效的登录尝试。");
            //        return View(userModels);
            //}
        }
        [HttpPost]
        public ActionResult Registe()
        {
            return RedirectToAction("Create", "UserModels");
        }
    }
}