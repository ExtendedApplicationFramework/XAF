namespace XAF.Modularity;
public interface IModuleManager
{
    bool Initialized { get; }

    Task DiscoverModules();

    IEnumerable<IModuleDescription> Modules { get; }

    Task LoadModules();

    Task StartLoadedModules();
}
