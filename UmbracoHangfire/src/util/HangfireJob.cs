using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoHangfire
{
    /// <summary>
    /// Give the hangfire job a free-text name
    /// </summary>
    public class HangfireJob : Attribute
    {
        private const int VERSION = 8;
        public string Name { get; set; }
        public HangfireJob(string Name)
        {
            this.Name = Name;
        }
    }
}