using Microsoft.Extensions.DependencyInjection;

namespace XAF.Modularity;
public abstract class ModuleHandler<TModule> : IModuleHandler
{
    public Type ModuleType { get; } = typeof(TModule);

    protected virtual Task LoadAsync(TModule module)
    {
        return Task.CompletedTask;
    }

    protected virtual Task StartAsync(TModule module)
    {
        return Task.CompletedTask;
    }

    public Task LoadAsync(object module)
    {
        return module is not TModule tModule 
            ? throw new NotSupportedException("") 
            : LoadAsync(tModule);
    }

    public Task StartAsync(object module)
    {
        return module is not TModule tModule 
            ? throw new NotSupportedException("") 
            : StartAsync(tModule);
    }

    public virtual Task ConfigureHandler(IModuleDescription description)
    {
        return Task.CompletedTask;
    }
}
