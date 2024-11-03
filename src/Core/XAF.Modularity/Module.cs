using System.ComponentModel;
using System.Reflection;
using XAF.Modularity.Catalogs;

namespace XAF.Modularity;

public class Module
{
    public Module(string name, string description, Version version, Type type, Assembly assembly, IModuleCatalog source)
    {
        Name = name;
        Description = description;
        Version = version;
        Type = type;
        Assembly = assembly;
        Source = source;
    }

    public string Name { get; }

    public string Description { get; }

    public Version Version { get; }

    public Type Type { get; }

    public Assembly Assembly { get; }

    public IModuleCatalog Source { get; }

    public IEnumerable<IModuleHandler> Handlers { get; set; } = Enumerable.Empty<IModuleHandler>();

    public static Module FromType(Type type, IModuleCatalog source)
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

}