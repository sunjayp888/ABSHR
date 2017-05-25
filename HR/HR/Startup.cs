using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HR.Startup))]
namespace HR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
