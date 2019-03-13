using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.Web;
using Umbraco.Core;
using Umbraco.Core.Security;
using Umbraco.Core.Composing;
using Umbraco.Web.Security;

namespace UmbracoHangfire
{
    /// <summary>
    /// Filter to ensure that only admin has access to the hangfire dashboard 
    /// </summary>
    public class HangfireAuthorisationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            bool result = false;
            var auth = new HttpContextWrapper(HttpContext.Current).GetUmbracoAuthTicket();
            if (auth != null)
            {
                var backofficeUser = Current.Services.UserService.GetByUsername(auth.Identity.Name);
                result = backofficeUser.Groups.FirstOrDefault(p => p.Alias == "admin") != null;
            }
            return result;
        }
    }
}
