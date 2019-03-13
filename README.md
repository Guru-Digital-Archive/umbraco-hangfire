# Hangfire scheduled tasks for Umbraco 8 #

This integrates https://github.com/HangfireIO/Hangfire with Umbraco v8 https://github.com/umbraco/Umbraco-CMS

## Installation ##
>### Manually ###
* Refer to Umbraco documentation to set up Umbraco development environment https://our.umbraco.com/download
* Add UmbracoHangfire project to your solution
* Add project reference to UmbracoHangfire
* Copy the contents of UmbracoHangfire\App_Plugins to the corresponding folder in your Umbraco project

## Umbraco Integration ##

* A Hangfire Tree node is added under Settings
** Jobs are listed
** Change Cron settings for any job
* The Hangfire Dashboard can be accessed by admin users at [yoururl]/umbraco/hangfire

## Example Usage ##

```csharp
[RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
public class DemoJob : IComposer
{
	private const string DemoJobID = "DemoJob";

	[HangfireJob("Demo Job")]
	public static void Execute()
	{
		// Add code to perform action here
		new HangfireDbContext().SaveHistory("Demo Job", "Demo Job Completed Successfully", DateTime.Now);            
	}

	/// <summary>
	/// Create job in startup of website
	/// </summary>
	public static void CreateRecurringJob()
	{
		// Create recurring job
		RecurringJobDto job = HangfireJobForm.JobFromId(DemoJobID);
		if (job == null)
		{
			RecurringJob.AddOrUpdate(DemoJobID, () => DemoJob.Execute(), "0 0 * * *", TimeZoneInfo.Local);
		}
	}

	public void Compose(Composition composition)
	{
		HangfireStartup.HangFireStarted += HangfireStartup_HangFireStarted;
	}

	private void HangfireStartup_HangFireStarted(object sender, HangfireStartedArgs e)
	{
		CreateRecurringJob();
	}
}
```