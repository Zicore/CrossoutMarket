using System;
using Crossout.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using NLog;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Crossout.Web
{
    public class Startup
    {
        public Startup()
        {
            //AppDomain.CurrentDomain.Load(typeof(TickerHub).Assembly.FullName);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Log.Error(args.ExceptionObject);
            };
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR(new HubConfiguration { EnableJSONP = true});
            app.UseNancy();
        }
    }
}
