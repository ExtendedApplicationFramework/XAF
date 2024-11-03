namespace XAF.Modularity;
public abstract class ModuleHandler<TModule> : IModuleHandler
{
    public Type ModuleType { get; } = typeof(TModule);

    public abstract Task LoadAsync(TModule module);

    public abstract Task StartAsync(TModule module);

    public Task LoadAsync(object module)
    {
        if (module is not TModule tModule)
        {
            throw new NotSupportedException($"Module {module} not supported form handler");
        }

        return LoadAsync(module);
    }

    public Task StartAsync(object module)
    {
        if (module is not TModule tModule)
        {
            throw new NotSupportedException($"Module {module} not supported form handler");
        }

        return StartAsync(module);
    }


}
