using System;
using System.Threading.Tasks;

namespace Example.Services.LoggingService
{
    public interface ILoggingService
    {
        ILogger GetLogger<T>();
        
        ILogger GetLogger<T>(string componentDescription);

        ILogger GetLogger<T>(Func<string> componentDescriptionFunc);

        ILogger GetLogger(Type componentType);
        
        ILogger GetLogger(Type componentType, string componentDescription);

        ILogger GetLogger(Type componentType, Func<string> componentDescriptionFunc);

        Task FlushAsync();
    }
}