using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cdc.Web.Startup))]
namespace Cdc.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
