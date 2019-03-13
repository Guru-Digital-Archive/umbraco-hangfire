using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Composing;
using Umbraco.Web;
using System;
using Hangfire;
using Owin;

namespace UmbracoHangfire
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class HangfireStartup : IComposer
    {
        public void Compose(Composition composition)
        {
            UmbracoDefaultOwinStartup.MiddlewareConfigured += UmbracoDefaultOwinStartup_MiddlewareConfigured;
        }

        private void UmbracoDefaultOwinStartup_MiddlewareConfigured(object sender, OwinMiddlewareConfiguredEventArgs e)
        {
            StartHangfireServer(e.AppBuilder);
            
            // Raise event for use by hangfire jobs
            OnHangfireStarted(new HangfireStartedArgs(e.AppBuilder));
        }

        public static event EventHandler<HangfireStartedArgs> HangFireStarted;

        internal static void OnHangfireStarted(HangfireStartedArgs args)
        {
            HangFireStarted?.Invoke(null, args);
        }

        /// <summary>
        /// Start up the hangfire server
        /// </summary>
        /// <param name="app"></param>
        public static void StartHangfireServer(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(
                    "umbracoDbDSN",
                    new Hangfire.SqlServer.SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(30) });
            app.UseHangfireDashboard("/umbraco/Hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorisationFilter() }
            });
            app.UseHangfireServer();
        }

    }

}