using XAF.Modularity.Catalogs;

namespace XAF.Modularity;
public class ModuleManager : IModuleManager
{
    protected Dictionary<Type, IModuleHandler> ModuleHandlers { get; }
    protected IEnumerable<IModuleCatalog> ModuleCatalogs { get; }
    protected ModuleManagerOptions? Options { get; }

    protected List<Module> ModulesInternal { get; } = [];
    protected List<(object instance, Module module)> LoadedModules { get; } = [];
    protected Func<Module, Task<object>> ModuleInstanceFactory { get; }

    public ModuleManager(IEnumerable<IModuleHandler> moduleHandlers, IEnumerable<IModuleCatalog> moduleCatalogs, ModuleManagerOptions options)
    {
        ModuleHandlers = moduleHandlers.ToDictionary(h => h.ModuleType);
        ModuleCatalogs = moduleCatalogs;
        Options = options;
        ModuleInstanceFactory = Options.ModuleInstanceFactory;
    }

    public IEnumerable<Module> Modules => ModulesInternal;

    public bool Initialized { get; protected set; }

    public virtual async Task Initialize()
    {
        foreach (var catalog in ModuleCatalogs)
        {
            ModulesInternal.AddRange(await catalog.GetModulesAsync(IsModule));
        }
        Initialized = true;
    }

    public virtual async Task LoadModules()
    {
        foreach (var module in ModulesInternal)
        {
            var moduleInstance = await ModuleInstanceFactory(module);
            module.Handlers ??= GetModuleHandlersFor(module.Type);

            if (!module.Handlers.Any())
            {
                throw new KeyNotFoundException($"Not handler for module of type {module.Name} found");
            }

            var tasks = module.Handlers.Select(h => h.LoadAsync(moduleInstance));
            await Task.WhenAll(tasks);
            LoadedModules.Add((moduleInstance, module));
        }
    }

    protected virtual IEnumerable<IModuleHandler> GetModuleHandlersFor(Type moduleType)
    {
        if (ModuleHandlers.ContainsKey(moduleType))
        {
            yield return ModuleHandlers[moduleType];
        }

        if (moduleType.IsClass)
        {
            var baseType = moduleType.BaseType;
            while (baseType != null)
            {
                if (ModuleHandlers.ContainsKey(baseType))
                {
                    yield return ModuleHandlers[baseType];
                }
            }
        }

        foreach (var itf in moduleType.GetInterfaces())
        {
            if (ModuleHandlers.ContainsKey(itf))
            {
                yield return ModuleHandlers[itf];
            }
        }
    }

    public virtual async Task StartLoadedModules()
    {
        foreach (var (instance, module) in LoadedModules)
        {
            var tasks = module.Handlers.Select(h => h.StartAsync(instance));
            await Task.WhenAll(tasks);
        }
    }

    protected virtual bool IsModule(Type type)
    {
        if (ModuleHandlers.ContainsKey(type))
        {
            return true;
        }

        if (type.IsClass)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (ModuleHandlers.ContainsKey(baseType))
                {
                    return true;
                }
            }
        }

        foreach (var itf in type.GetInterfaces())
        {
            if (ModuleHandlers.ContainsKey(itf))
            {
                return true;
            }
        }

        return false;
    }
}
