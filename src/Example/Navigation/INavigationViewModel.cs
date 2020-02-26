using Example.Services.LoggingService;
using ReactiveUI;

namespace Example.Navigation
{
    public interface INavigationViewModel : IHaveActivator, IHaveLogger, IReactiveObject
    {
    }
}