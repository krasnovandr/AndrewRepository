using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AudioNetwork.Startup))]
namespace AudioNetwork
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
