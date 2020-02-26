using Example.Data;
using Example.Model;
using Example.Services.AuthService;
using Example.Services.CrashlyticsService;
using Example.Utils;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Services.LoggingService
{
    public class LoggingService : ILoggingService
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ICrashlyticsService crashlyticsService;
        private readonly Lazy<IDatabase> db;
        private readonly IScheduler scheduler;
        private readonly ILogger logger;
        private readonly ConcurrentQueue<Log> logsToSave = new ConcurrentQueue<Log>();
        
        private readonly SemaphoreSlim flushSemaphore = new SemaphoreSlim(1, 1);

        public LoggingService(IReadonlyDependencyResolver dr)
        {
            crashlyticsService = dr.GetService<ICrashlyticsService>();
            db = new Lazy<IDatabase>(() => dr.GetService<IDatabase>());
            scheduler = dr.GetService<IScheduler>() ?? RxApp.MainThreadScheduler;
            AuthService = new Lazy<IAuthService>(() => dr.GetService<IAuthService>());
            logger = GetLogger<LoggingService>();
        }
        
        internal Lazy<IAuthService> AuthService { get; }

        internal DateTimeOffset Now => scheduler.Now;

        public ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public ILogger GetLogger<T>(string componentDescription)
        {
            return GetLogger(typeof(T), componentDescription);
        }

        public ILogger GetLogger<T>(Func<string> componentDescriptionFunc)
        {
            return GetLogger(typeof(T), componentDescriptionFunc);
        }

        public ILogger GetLogger(Type componentType)
        {
            return new Logger(componentType, this);
        }

        public ILogger GetLogger(Type componentType, string componentDescription)
        {
            return new Logger(componentType, componentDescription, this);
        }

        public ILogger GetLogger(Type componentType, Func<string> componentDescriptionFunc)
        {
            return new Logger(componentType, componentDescriptionFunc, this);
        }

        internal void LogInternal(Log log)
        {
            logsToSave.Enqueue(log);

            var message = GetString(log);
            Debug.WriteLine(message);
            #if !DEBUG
            crashlyticsService.Log(message);
            #endif
        }

        public async Task FlushAsync()
        {
            using (await flushSemaphore.UseWaitAsync())
            {
                logger.Debug(nameof(FlushAsync));
                var savingLogs = new List<Log>();
                while (logsToSave.TryDequeue(out var currentLog))
                {
                    savingLogs.Add(currentLog);
                }
                
                await db.Value.BulkInsertAsync(savingLogs).ConfigureAwait(false);
            }
        }

        private static string GetString(Log log)
        {
            var sb = new StringBuilder($"[{log.ComponentType}");
            if (log.ComponentDescription != null)
            {
                sb.Append($"({log.ComponentDescription})");
            }

            sb.Append("]");
            
            if (log.LogType == LogType.UserAction)
            {
                sb.Append("[UserAction]");
            }

            sb.Append($"[ThreadId={log.ThreadId}]");

            sb.Append($"{log.DateTime:dd.MM.yyyy HH:mm:ss}: {log.Message}");
            return sb.ToString();
        }
    }
}