namespace XAF.Modularity;
public interface IModuleHandler
{
    Type ModuleType { get; }

    Task ConfigureHandler(IModuleDescription description);

    Task LoadAsync(object module);

    Task StartAsync(object module);
}
