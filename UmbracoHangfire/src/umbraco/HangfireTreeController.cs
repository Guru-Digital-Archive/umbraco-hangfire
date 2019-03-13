using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Core;

namespace UmbracoHangfire
{
    [PluginController("UmbracoHangfire")]
    [Tree("settings", HangfireConstants.TreeAlias, SortOrder = 1)]
    public class HangfireTreeController : TreeController
    {

        List<RecurringJobDto> RecurringJobs = Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs();

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            MenuItemCollection result = new MenuItemCollection();
            return result;
        }

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var node = base.CreateRootNode(queryStrings);
            node.MenuUrl = null; // Stops ... appearing after the root node
            return node;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            TreeNodeCollection result = new TreeNodeCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
                var cleanNode = CreateTreeNode(HangfireConstants.CleanAlias, id, queryStrings, HangfireConstants.CleanName, "icon-brush-alt", false, String.Format("{0}/{1}/clean/-1", "settings", HangfireConstants.TreeAlias));
                cleanNode.MenuUrl = null;
                result.Add(cleanNode);

                var jobsNode = CreateTreeNode(HangfireConstants.JobsAlias, id, queryStrings, HangfireConstants.JobsName, "icon-lab", RecurringJobs.Count > 0, String.Format("{0}/{1}/jobs/-1", "settings", HangfireConstants.TreeAlias));
                jobsNode.MenuUrl = null;
                result.Add(jobsNode);

            }
            else if (id == HangfireConstants.JobsAlias && RecurringJobs.Count > 0)
            {
                int i = 0, j = 0;
                string icon, name;
                foreach (RecurringJobDto job in RecurringJobs)
                {
                    if (job.Job == null) continue;

                    if (job.LastJobState == SucceededState.StateName || job.LastJobState == EnqueuedState.StateName)
                        icon = "icon-play color-green";
                    else if (job.LastJobState == ProcessingState.StateName)
                        icon = "icon-play color-yellow";
                    else if (job.LastJobState == FailedState.StateName)
                        icon = "icon-play color-red";
                    else if (job.LastJobState == DeletedState.StateName)
                        icon = "icon-stop color-red";
                    else
                        icon = "icon-play";

                    if (job.LastJobState != DeletedState.StateName)
                    {
                        HangfireJob[] attribs = (HangfireJob[])job.Job.Method.GetCustomAttributes(typeof(HangfireJob), false);

                        if (attribs.Length > 0)
                            name = attribs[0].Name;
                        else
                            name = "Untitled job " + (++j);
                    }
                    else name = "Deleted";

                    result.Add(CreateTreeNode(job.Id, id, queryStrings, name, icon, false, String.Format("{0}/{1}/job/{2}", "settings", HangfireConstants.TreeAlias, job.Id)));
                    i++;
                }
            }
            return result;
        }
    }
}