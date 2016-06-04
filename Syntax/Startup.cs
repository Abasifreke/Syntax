using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Syntax.Startup))]
namespace Syntax
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
