using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EthnoBot.Startup))]
namespace EthnoBot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
