using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Improview.Startup))]
namespace Improview
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
