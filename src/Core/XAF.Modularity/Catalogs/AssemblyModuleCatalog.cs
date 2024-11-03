using System.Reflection;
using XAF.Modularity.Context;

namespace XAF.Modularity.Catalogs;
public class AssemblyModuleCatalog : IModuleCatalog
{
    private Assembly? _assembly;
    private readonly AssemblyModuleCatalogOptions _options;
    private readonly string _assemblyPath;
    private Module[]? _modules;

    public AssemblyModuleCatalog(Assembly assembly, AssemblyModuleCatalogOptions? options = null)
    {
        _assembly = assembly;
        _options = options ?? new();
        _assemblyPath = assembly.Location;
    }

    public AssemblyModuleCatalog(string assemblyPath, AssemblyModuleCatalogOptions? options = null)
    {
        _assemblyPath = assemblyPath;
        _options = options ?? new();
    }

    public Task<Module[]> GetModulesAsync(Func<Type, bool> typeMatch)
    {
        var modules = new List<Module>();
        if (_modules is not null)
        {
            return Task.FromResult(_modules);
        }

        if (_assembly is null)
        {
            if (!File.Exists(_assemblyPath))
            {
                throw new ArgumentException($"Assembly in path {_assemblyPath} does not exist.");
            }

            var contextLoader = new ModuleContextLoader(_assemblyPath, _options.ContextLoaderOptions);
            _assembly = contextLoader.Load();

            if (_assembly is null)
            {
                throw new Exception($"could not load assembly from path '{_assemblyPath}'");
            }
        }

        foreach (var type in _assembly.GetExportedTypes())
        {
            if (!typeMatch(type))
            {
                continue;
            }

            modules.Add(Module.FromType(type, this));
        }

        _modules = modules.ToArray();

        return Task.FromResult(_modules);
    }
}
