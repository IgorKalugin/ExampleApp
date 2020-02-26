using Example.Navigation;
using ReactiveUI;

namespace Example.ViewModels.Pages.Profile
{
    public interface IProfileSectionViewModel : IHaveActivator, IReactiveObject
    {
        string Name { get; }
    }
}