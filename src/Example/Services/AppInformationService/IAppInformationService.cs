namespace Example.Services.AppInformationService
{
    public interface IAppInformationService
    {
        string Version { get; }
        
        string VersionBuild { get; }
    }
}