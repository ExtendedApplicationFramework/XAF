using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using XAF.Modularity.Catalogs;

namespace XAF.Modularity;
public class ModuleManager : IModuleManager
{
    private readonly ILogger<ModuleManager> _logger;

    protected Dictionary<Type, IModuleHandler> ModuleHandlers { get; }
    protected IEnumerable<IModuleCatalog> ModuleCatalogs { get; }

    private List<IModuleDescription> _discoveredModules = [];
    protected List<IModuleDescription> LoadedModules { get; } = [];

    public ModuleManager(
        IEnumerable<IModuleHandler> moduleHandlers,
        IEnumerable<IModuleCatalog> moduleCatalogs,
        ILogger<ModuleManager> logger)
    {
        ModuleHandlers = moduleHandlers.ToDictionary(h => h.ModuleType);
        ModuleCatalogs = moduleCatalogs;
        _logger = logger;
    }

    public IEnumerable<IModuleDescription> Modules => _discoveredModules;

    public bool Initialized { get; protected set; }

    public virtual async Task DiscoverModules()
    {
        foreach (var catalog in ModuleCatalogs)
        {
            _discoveredModules.AddRange(await catalog.GetModulesAsync(IsModule));
        }
        Initialized = true;
    }

    public virtual async Task LoadModules()
    {
        if (!Initialized)
        {
            throw new InvalidOperationException("Call Initialize first");
        }

        foreach (var moduleDescription in _discoveredModules)
        {
            _logger.LogDebug("Loading Module '{ModuleName}'", moduleDescription.Name);

            moduleDescription.Handlers.AddRange(GetModuleHandlersFor(moduleDescription.ModuleType));

            if (!moduleDescription.Handlers.Any())
            {
                _logger.LogError("No handler for module '{Name} ({Type})' found", moduleDescription.Name, moduleDescription.ModuleType);
                continue;
            }
            var configTasks = moduleDescription.Handlers.Select(h => h.ConfigureHandler(moduleDescription));

            await Task.WhenAll(configTasks);

            if (moduleDescription.Instance is null)
            {
                var moduleInstance = CreateModule(moduleDescription);
                moduleDescription.SetInstance(moduleInstance);
            }

            _logger.LogDebug("Module instance for module '{ModuleName}' created", moduleDescription.Name);

            _logger.LogDebug(
               "The following module handlers are used for module '{ModuleName}':\n\t{Handlers}",
               moduleDescription.Name,
               moduleDescription.Handlers.Select(h => h.GetType().FullName! + "\n\t"));


            var tasks = moduleDescription.Handlers.Select(h => h.LoadAsync(moduleDescription.Instance!));
            await Task.WhenAll(tasks);
            LoadedModules.Add(moduleDescription);
            _logger.LogDebug("Module '{ModuleName}' Loaded", moduleDescription.Name);
        }
    }

    protected virtual List<IModuleHandler> GetModuleHandlersFor(Type moduleType)
    {
        var handlers = new List<IModuleHandler>();

        if (ModuleHandlers.ContainsKey(moduleType))
        {
            handlers.Add(ModuleHandlers[moduleType]);
        }

        if (moduleType.IsClass)
        {
            var baseType = moduleType.BaseType;
            while (baseType != null)
            {
                if (ModuleHandlers.TryGetValue(baseType, out var value))
                {
                    handlers.Add(value);
                    break;
                }
                baseType = baseType.BaseType;
            }
        }

        foreach (var itf in moduleType.GetInterfaces())
        {
            if (ModuleHandlers.TryGetValue(itf, out var value))
            {
                handlers.Add(value);
            }
        }

        return handlers;
    }

    public virtual async Task StartLoadedModules()
    {
        foreach (var module in LoadedModules)
        {
            var tasks = module.Handlers.Select(h => h.StartAsync(module.Instance!));
            await Task.WhenAll(tasks);
        }
    }

    protected virtual object CreateModule(IModuleDescription moduleDescription)
    {
        var ctor = moduleDescription.ModuleType.GetConstructor(Type.EmptyTypes);

        return ctor == null
            ? throw new NotSupportedException("The default module manager can't handle Modules with non empty constructors")
            : ctor.Invoke([]);
    }

    protected virtual bool IsModule(Type type)
    {
        if (type.IsAbstract || type.IsInterface)
        {
            return false;
        }

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
                baseType = baseType.BaseType;
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
