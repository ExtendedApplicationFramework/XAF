namespace XAF.Modularity;
public interface IModuleHandler
{
    Type ModuleType { get; }

    Task LoadAsync(object module);

    Task StartAsync(object module);
}
