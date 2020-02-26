using System;
using System.Threading;
using Example.Model;

namespace Example.Services.LoggingService
{
    public class Logger : ILogger
    {
        private readonly string typeName;
        private readonly string componentDescription;
        private readonly Func<string> componentDescriptionFunc;
        private readonly LoggingService loggingService;

        public Logger(Type componentType, LoggingService loggingService)
        { 
            typeName = componentType.Name;
            this.loggingService = loggingService;
        }

        public Logger(Type componentType, string componentDescription, LoggingService loggingService) : this(componentType, loggingService)
        {
            this.componentDescription = componentDescription;
        }

        // this type of ctor can be used when componentDescription of the logger can be changed dynamically at runtime
        public Logger(Type componentType, Func<string> componentDescriptionFunc, LoggingService loggingService) : this(componentType, loggingService)
        {
            this.componentDescriptionFunc = componentDescriptionFunc;
        }

        public void Error(string message)
        {
            var log = GetLog(message, LogType.Error);
            loggingService.LogInternal(log);
        }

        public void Warn(string message)
        {
            var log = GetLog(message, LogType.Warn);
            loggingService.LogInternal(log);
        }

        public void Debug(string message)
        {
            var log = GetLog(message, LogType.Debug);
            loggingService.LogInternal(log);
        }
        
        public void UserAction(string message)
        {
            var log = GetLog(message, LogType.UserAction);
            loggingService.LogInternal(log);
        }

        private Log GetLog(string message, LogType logType)
        {
            return new Log
            {
                DateTime = loggingService.Now, ComponentType = typeName, ComponentDescription = GetName(), Message = message, LogType = logType,
                User = loggingService.AuthService.IsValueCreated ? loggingService.AuthService.Value.CurrentUser : null,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
        }

        private string GetName()
        {
            return componentDescription ?? componentDescriptionFunc?.Invoke();
        }
    }
}