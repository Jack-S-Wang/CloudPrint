using Microsoft.Owin;
using Owin;
using CloudPrinter.Migrations;
using CloudPrinter.Models;
using System.Data.Entity;
using CloudPrinter.TCPServer;
using Z.EntityFramework.Plus;

[assembly: OwinStartupAttribute(typeof(CloudPrinter.Startup))]
namespace CloudPrinter
{
    public partial class Startup
    {
        private static tcpServer sertcp = null;
        public void Configuration(IAppBuilder app)
        {
            sertcp = new tcpServer();
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
            ConfigureAuth(app);
            var db = new ApplicationDbContext();
            db.PrinterModels.Update(x => new PrinterModels { mState = false });
            //db.PrinterModels.SqlQuery("update dbo.PrinterModels set mState=0").FirstAsync();
        }
    }
}
