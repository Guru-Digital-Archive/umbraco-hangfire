using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoHangfire
{
    /// <summary>
    /// Selections for Cron scheduler
    /// </summary>
    public enum HangfireMod
    {
        Minute,
        Hour,
        Day,
        Month
    }
}