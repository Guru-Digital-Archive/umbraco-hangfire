using Owin;

namespace UmbracoHangfire
{
    /// <summary>
    /// Arguments for the startup event to pass AppBuilder to jobs
    /// </summary>
    public class HangfireStartedArgs
    {
        public HangfireStartedArgs(IAppBuilder appBuilder)
        {
            AppBuilder = appBuilder;
        }
        public IAppBuilder AppBuilder { get; private set; }
    }
}