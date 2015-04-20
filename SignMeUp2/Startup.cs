using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SignMeUp2.Startup))]
namespace SignMeUp2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
