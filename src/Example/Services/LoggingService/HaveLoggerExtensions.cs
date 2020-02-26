using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Example.Services.AnalyticsInfrastructure;
using ReactiveUI;
using Splat;

namespace Example.Services.LoggingService
{
    public static class HaveLoggerExtensions
    {
        public static Lazy<ILogger> GetLogger<TSender>(this TSender sender, IReadonlyDependencyResolver dr)
        {
            return new Lazy<ILogger>(() => dr.GetService<ILoggingService>().GetLogger<TSender>());
        }
        
        public static Lazy<ILogger> GetLogger<TSender>(this TSender sender, IReadonlyDependencyResolver dr, string componentName)
        {
            return new Lazy<ILogger>(() => dr.GetService<ILoggingService>().GetLogger<TSender>(componentName));
        }
        
        public static Lazy<ILogger> GetLogger<TSender>(this TSender sender, IReadonlyDependencyResolver dr, Func<string> componentDescriptionFunc)
        {
            return new Lazy<ILogger>(() => dr.GetService<ILoggingService>().GetLogger<TSender>(componentDescriptionFunc));
        }
        
        public static Lazy<ILogger> GetLogger<TSender>(this TSender sender, IReadonlyDependencyResolver dr, Type type)
        {
            return new Lazy<ILogger>(() => dr.GetService<ILoggingService>().GetLogger(type));
        }
        
        public static void LogPropertyChange<TSender, TValue>(this TSender sender, string propertyName, TValue value, Func<TValue, string> format = null)
            where TSender : IHaveLogger
        {
            var valueFormatted = format != null ? format(value) : value.ToString();
            sender.Logger.Value.UserAction($"{propertyName} changed to: {valueFormatted}");
        }
        
        public static IDisposable LogPropertyChange<TSender, TRet>(this TSender sender, Expression<Func<TSender, TRet>> expression, string propertyName = null, Func<TRet, string> format = null)
            where TSender : IHaveLogger
        {
            propertyName = propertyName ?? expression.Body.GetMemberInfo().Name;
            return sender.WhenAnyValue(expression)
                .Skip(1)
                .Subscribe(value => sender.LogPropertyChange(propertyName, value, format));
        }
        
        public static IDisposable LogPropertyChangeThrottling<TSender, TRet>(this TSender sender, Expression<Func<TSender, TRet>> expression, string propertyName = null, Func<TRet, string> format = null, IScheduler scheduler = null)
            where TSender : IHaveLogger
        {
            propertyName = propertyName ?? expression.Body.GetMemberInfo().Name;
            scheduler = scheduler ?? DefaultScheduler.Instance;
            return sender.WhenAnyValue(expression)
                .Skip(1)
                .Throttle(TimeSpan.FromSeconds(1), scheduler)
                .Subscribe(value => sender.LogPropertyChange(propertyName, value, format));
        }

        /// <summary>
        /// Logs command invocation including thrown exceptions
        /// </summary>
        /// <remarks><paramref name="logger"/> shouldn't be able to outlive the command!<para/>
        /// <paramref name="logger"/> subscribes to ThrownExceptions, so you shouldn't use <see cref="LogInvocation{TInput,TOutput}"/> outside of its view model
        /// </remarks>
        public static ReactiveCommand<TInput, TOutput> LogInvocation<TInput, TOutput>(this ReactiveCommand<TInput, TOutput> command, string name, Lazy<ILogger> logger, IReadonlyDependencyResolver dr,
            Func<TInput, string> format = null)
        {
            var wrapperCommand = ReactiveCommand.CreateFromTask<TInput, TOutput>(async input =>
            {
                var logValue = format != null ? $"{name}: {format(input)}" : name;
                logger.Value.UserAction(logValue);
                var analyticServers = dr.GetServices<IAnalyticsServer>();

                foreach (var analyticServer in analyticServers)
                    analyticServer.LogEvent(name);

                return await command.Execute(input);
            }, command.CanExecute);
            wrapperCommand.ThrownExceptions.Subscribe(ex => { logger.Value.Error($"{name} exception: {ex}"); });
            return wrapperCommand;
        }
    }
}