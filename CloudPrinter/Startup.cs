using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CloudPrinter.Startup))]
namespace CloudPrinter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
