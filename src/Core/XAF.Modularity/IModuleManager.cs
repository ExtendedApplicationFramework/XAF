namespace XAF.Modularity;
public interface IModuleManager
{
    IObservable<Module> ModuleLoading();
    IObservable<Module> ModuleLoaded();
    IObservable<Module> ModuleStarting();
    IObservable<Module> ModuleStarted();

    bool Initialized { get; }

    Task Initialize();

    IEnumerable<Module> Modules { get; }

    Task LoadModules();

    Task StartLoadedModules();
}
