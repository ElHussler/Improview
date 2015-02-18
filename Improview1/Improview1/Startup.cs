using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Improview1.Startup))]
namespace Improview1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
