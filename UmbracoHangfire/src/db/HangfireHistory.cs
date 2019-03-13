using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UmbracoHangfire
{
    /// <summary>
    /// Record of running of a hangfire job
    /// </summary>
    public class HangfireHistory 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HangfireId { get; set; }
        public string Message { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }

        public static HangfireHistory Create(string hangfireId, string message, DateTime startDate)
        {
            DateTime today = DateTime.Now;
            HangfireHistory result = new HangfireHistory
            {
                Duration = (int)(today - startDate).TotalSeconds,
                HangfireId = hangfireId,
                Message = message,
                StartTime = startDate
            };
            return result;
        }


    }
}