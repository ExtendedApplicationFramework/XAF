using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reflection;
using XAF.Modularity.Catalogs;

namespace XAF.Modularity;

internal class ModuleDescription : IModuleDescription
{
    public ModuleDescription(string name, string description, Version version, Type type, Assembly assembly, IModuleCatalog source)
    {
        Name = name;
        Description = description;
        Version = version;
        ModuleType = type;
        Assembly = assembly;
        Source = source;
    }

    public string Name { get; }

    public string Description { get; }

    public Version Version { get; }

    public Type ModuleType { get; }

    public Assembly Assembly { get; }

    public IModuleCatalog Source { get; }

    public IList<ServiceDescriptor> Services { get; } = [];

    public List<IModuleHandler> Handlers { get; } = [];

    public object? Instance { get; set; }

    public static ModuleDescription FromType(Type type, IModuleCatalog source)
    {
        var name = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
        var description = type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
        var version = type.Assembly.GetName().Version ?? new(1, 0, 0);

        return new(
            name,
            description,
            version,
            type,
            type.Assembly,
            source);
    }

    public bool SetInstance(object instance)
    {
        if(Instance is null)
        {
            Instance = instance;
            return true;
        }

        return false;
    }
}