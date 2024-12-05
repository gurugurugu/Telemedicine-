using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Telemedicine.Startup))]
namespace Telemedicine
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}