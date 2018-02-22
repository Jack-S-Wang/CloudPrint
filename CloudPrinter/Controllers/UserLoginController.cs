using CloudPrinter.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudPrinter.Controllers
{
    public class UserLoginController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: UserLogin
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost, ActionName("LoginIn")]
        public ActionResult LoginIn(FormCollection collection)
        {
            string userName = collection["userName"];
            string password = collection["password"];
            if (userName != "")
            {
                //var query = from u in db.UserModels
                //            where u.userName.Equals(userName)
                //            select u;

                //var queryString = query.ToString();

                //var um = query.First();

                UserModels um=db.UserModels.SqlQuery("select * from dbo.UserModels where userName=@userName",new SqlParameter("@userName",userName)).First();
                //db.Database.Log = Console.Write(true);
                if (um != null)
                {
                    if (password.Equals(um.password))
                    {
                        return RedirectToAction("Index", "PrinterModels",um.UserModelsId);
                    }
                    else
                    {
                        return HttpNotFound("Password is worng!");
                    }
                }
                else
                {
                    return HttpNotFound("No User");
                }
                
            }else
            {
                return HttpNotFound("No User");
            }
            
        }
    }
}