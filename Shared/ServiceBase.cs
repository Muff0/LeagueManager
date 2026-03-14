using Microsoft.Extensions.Logging;

namespace Shared
{
    public class ServiceBase
    {
        protected ILogger Logger { get; }

        public ServiceBase(ILogger logger)
        {
            Logger = logger;
        }
        
        protected virtual void HandleException(Exception e)
        {
            if (Logger != null)
            {
                Logger.LogCritical(e, e.Message);
            }
            
        }
    }
}
