using UmbracoHangfire;
using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.WebApi;

namespace UmbracoHangfirePlugin
{
    /// <summary>
    /// Ref: https://our.umbraco.org/documentation/reference/routing/Authorized/
    /// Auto routed to /umbraco/backoffice/api/{controller}/{action}
    /// </summary>
    public class HangfireApiController : UmbracoAuthorizedApiController
    {

        [HttpGet]
        public object GetAllJobs()
        {
            return
                from job in Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs()
                select new HangfireJobForm(job);
        }

        [HttpGet]
        public object GetSettings(string id)
        {
            RecurringJobDto job = HangfireJobForm.JobFromId(id);
            HangfireJobForm result = null;
            if (job != null)
            {
                result = new HangfireJobForm(job);
            }
            return result;
        }

        [HttpPost]
        public object ExecuteNow(HangfireJobForm data)
        {
            RecurringJobManager manager = new RecurringJobManager();
            RecurringJobDto job = HangfireJobForm.JobFromId(data.Id);
            manager.Trigger(job.Id);
            job = HangfireJobForm.JobFromId(data.Id);

            return new
            {
                data.Id,
                job.Cron,
                Success = true,
                Message = "Hangfire job is now queued for execution.",
                LastExecuted = HangfireJobForm.GetLocalDateString(job.LastExecution),
                NextExecution = HangfireJobForm.GetLocalDateString(job.NextExecution)
            };
        }

        public object Save(HangfireJobForm data)
        {
            RecurringJobManager manager = new RecurringJobManager();
            RecurringJobDto job = HangfireJobForm.JobFromId(data.Id);
            manager.RemoveIfExists(data.Id);
            manager.AddOrUpdate(data.Id, job.Job, data.Cron, TimeZoneInfo.Local);
            job = HangfireJobForm.JobFromId(data.Id);
            while (job.NextExecution == null)
            {
                // Wait for the update to be stored in Hangfire's database
                System.Threading.Thread.Sleep(2000);
                job = HangfireJobForm.JobFromId(data.Id);
            }
            return new
            {
                Success = true,
                Message = "Settings were saved successfully!",
                LastExecuted = HangfireJobForm.GetLocalDateString(job.LastExecution),
                NextExecution = HangfireJobForm.GetLocalDateString(job.NextExecution)
            };
        }

        [HttpPost]
        public object CleanOrphaned()
        {
            List<RecurringJobDto> recurringJobs = Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs();
            RecurringJobManager manager = new RecurringJobManager();
            int i = 0;
            foreach (RecurringJobDto dto in recurringJobs)
                if (dto.Job == null)
                {
                    i++;
                    manager.RemoveIfExists(dto.Id);
                }

            string message = i < 1 ? "There were no orphaned jobs to clean" : "Cleaned " + i + " orphaned jobs.";

            return new
            {
                Success = i > 0,
                Message = message
            };
        }
    }
}