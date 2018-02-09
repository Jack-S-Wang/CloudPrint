using Microsoft.Owin;
using Owin;
using CloudPrinter.Migrations;
using CloudPrinter.Models;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(CloudPrinter.Startup))]
namespace CloudPrinter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            ConfigureAuth(app);
        }
    }
}
