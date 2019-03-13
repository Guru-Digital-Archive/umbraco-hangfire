using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoHangfire
{
    /// <summary>
    /// Helper class to pass to/from AJAX calls
    /// </summary>
    public class HangfireJobForm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Cron { get; set; }
        public int CronNum { get; set; }
        public string CronMod { get; set; }
        public string State { get; set; }
        public string LastExecuted { get; set; }
        public string NextExecution { get; set; }

        public HangfireJobForm() { }

        public HangfireJobForm(RecurringJobDto job)
        {
            Id = job.Id;
            Name = ((HangfireJob[])job.Job.Method.GetCustomAttributes(typeof(HangfireJob), false)).Length > 0
            ? ((HangfireJob[])job.Job.Method.GetCustomAttributes(typeof(HangfireJob), false))[0].Name : "Untitled";
            Cron = job.Cron;
            CronNum = new HangfireNumMod(job.Cron).Num;
            CronMod = new HangfireNumMod(job.Cron).ModToString();
            State = (job.LastJobState ?? "None");
            LastExecuted = GetLocalDateString(job.LastExecution);
            NextExecution = GetLocalDateString(job.NextExecution);

        }

        public static string GetLocalDateString(DateTime? utcDate)
        {
            string result = "N/A";
            if (utcDate.HasValue)
            {
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value, TimeZoneInfo.Local);
                result = localDate.ToString("yyyy-MM-dd HH:mm");
            }
            return result;
        }

        public static RecurringJobDto JobFromId(string id)
        {
            RecurringJobDto job =
                 Hangfire.JobStorage.Current.GetConnection()
                 .GetRecurringJobs()
                 .FirstOrDefault(p => p.Id == id);
            return job;
        }



    }
}