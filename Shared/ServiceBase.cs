using Microsoft.Extensions.Logging;

namespace Shared;

public class ServiceBase
{
    public ServiceBase(ILogger logger)
    {
        Logger = logger;
    }

    protected ILogger Logger { get; }

    protected virtual void HandleException(Exception e)
    {
        if (Logger != null) Logger.LogCritical(e, e.Message);
    }
}