using System;

namespace Example.Services.LoggingService
{
    public interface IHaveLogger
    {
        Lazy<ILogger> Logger { get; }
    }
}