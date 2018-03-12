using CloudPrinter.Models;
using CloudPrinter.SharClass;
using CloudPrinter.superAdmin;
using CloudPrinter.TCPServer;
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
        private static tcpServer sertcp=new tcpServer();
        private static TcpDataPrintServer tcpDataSer=new TcpDataPrintServer();
        // GET: UserLogin
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Client, NoStore = false, Duration = 30)]
        public ActionResult LoginIn([Bind(Include = "userAccount,password")] UserModels userModels, string code)
        {
            if (ModelState.IsValid)
            {
                if (Session["vcode"].Equals(code.ToLower()))
                {
                    var um = db.UserModels.Find(userModels.userAccount);
                    if (um != null)
                    {
                        if (um.password.Equals(userModels.password))
                        {
                            db.Entry(um).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            HttpCookie cookieLogin = new HttpCookie("LoginAccount", userModels.userAccount);
                            if (Request.Cookies.Get("LoginAccount") != null)
                            {
                                Response.Cookies.Set(cookieLogin);
                            }
                            else
                            {
                                Response.Cookies.Add(cookieLogin);
                            }
                            Response.Cookies["LoginAccount"].Expires = DateTime.Now.AddHours(2);
                            if (userModels.userAccount.Equals(AdminInfo.ADMINACCOUNT))
                            {
                                return RedirectToAction("Index", "UserModels");
                            }
                            return RedirectToAction("Index", "PrinterModels", new { userAccount = userModels.userAccount });
                        }
                        else
                        {
                            ModelState.AddModelError("password", "密码错误！");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("userAccount", "未找到对应的账号！");
                    }
                }
                ModelState.AddModelError("userAccount", "验证码出错！");
            }
            return View("Login",userModels);
        }
        [HttpPost]
        public ActionResult Registe()
        {
            return RedirectToAction("Create", "UserModels");
        }

        #region 验证码
        [OutputCache(Duration = 0)]
        public ActionResult VCode()
        {
            string code = ValidateCode.CreateRandomCode(4);
            Session["vcode"] = code.ToLower();
            using (var ms = ValidateCode.CreateImage(code))
            {
                return File(ms.GetBuffer(), "image/jpeg");
            }
        }
        #endregion

    }
}