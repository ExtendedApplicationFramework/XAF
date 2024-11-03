using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace XAF.Modularity.Catalogs;
public class FolderModuleCatalog : IModuleCatalog
{
    private readonly string _folderPath;
    private readonly FolderModuleCatalogOptions _options;

    public FolderModuleCatalog(string folderPath, FolderModuleCatalogOptions? options = null)
    {
        _folderPath = folderPath;
        _options = options ?? new FolderModuleCatalogOptions();
    }

    public async Task<Module[]> GetModulesAsync(Func<Type, bool> typeMatch)
    {
        var files = new List<string>();

        foreach (var searchPattern in _options.SearchPatterns)
        {
            var dllFiles = Directory.GetFiles(
                _folderPath,
                searchPattern,
                _options.IncludeSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            files.AddRange(dllFiles);
        }

        var assemblyCatalogOptions = new AssemblyModuleCatalogOptions()
        {
            ContextLoaderOptions = _options.ContextLoaderOptions
        };

        var initializer = files.Distinct()
            .Where(p => IsModuleAssembly(p, typeMatch))
            .Select(p => new AssemblyModuleCatalog(p, assemblyCatalogOptions).GetModulesAsync(typeMatch));

        var modules = await Task.WhenAll(initializer);
        return modules.SelectMany(m => m).ToArray();
    }

    private bool IsModuleAssembly(string assemblyPath, Func<Type, bool> typeMatch)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var reader = new PEReader(stream);

        if (!reader.HasMetadata)
        {
            return false;
        }

        var runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
        var runtimeAssemblies = Directory.GetFiles(runtimeDirectory, "*.dll");
        var paths = new List<string>(runtimeAssemblies) { assemblyPath };

        if (_options.ContextLoaderOptions.AdditionalRuntimePaths.Any())
        {
            foreach (var path in _options.ContextLoaderOptions.AdditionalRuntimePaths)
            {
                var dlls = Directory.GetFiles(path, "*.dll");
                paths.AddRange(dlls);
            }
        }

        switch (_options.ContextLoaderOptions.ResolveAssembliesFromHost)
        {
            case Context.ResolveAssembliesFromHost.Never:
                var modulePath = Path.GetDirectoryName(assemblyPath);
                var dllsInPluginPath = Directory.GetFiles(modulePath, "*.dll", SearchOption.AllDirectories);

                paths.AddRange(dllsInPluginPath);
                break;
            case Context.ResolveAssembliesFromHost.Always:
                var hostApplicationPath = Environment.CurrentDirectory;
                var hostDlls = Directory.GetFiles(hostApplicationPath, "*.dll", SearchOption.AllDirectories);

                paths.AddRange(hostDlls);

                AddSharedFrameworkDlls(hostApplicationPath, runtimeDirectory, paths);
                break;
            case Context.ResolveAssembliesFromHost.PreferPlugin:
                break;
        }

        paths = paths
            .Select(x => new { FullPath = x, FileName = Path.GetFileName(x) })
            .GroupBy(x => x.FileName)
            .Select(x => x.First().FullPath)
            .ToList();

        var resolver = new PathAssemblyResolver(paths);

        using var metadataContext = new MetadataLoadContext(resolver);
        var assembly = metadataContext.LoadFromAssemblyName(assemblyPath);

        return assembly.GetExportedTypes().Any(typeMatch);

    }

    private void AddSharedFrameworkDlls(string hostApplicationPath, string runtimeDirectory, List<string> paths)
    {
        var defaultAssemblies = AssemblyLoadContext.Default.Assemblies.ToList();

        var defaultAssemblyDirectories = defaultAssemblies.Where(x => x.IsDynamic == false).Where(x => string.IsNullOrWhiteSpace(x.Location) == false)
            .GroupBy(x => Path.GetDirectoryName(x.Location)).Select(x => x.Key).ToList();

        foreach (var assemblyDirectory in defaultAssemblyDirectories)
        {
            if (string.Equals(assemblyDirectory.TrimEnd('\\').TrimEnd('/'), hostApplicationPath.TrimEnd('\\').TrimEnd('/')))
            {
                continue;
            }

            if (string.Equals(assemblyDirectory.TrimEnd('\\').TrimEnd('/'), runtimeDirectory.TrimEnd('\\').TrimEnd('/')))
            {
                continue;
            }

            if (_options.ContextLoaderOptions.AdditionalRuntimePaths == null)
            {
                _options.ContextLoaderOptions.AdditionalRuntimePaths = new List<string>();
            }

            if (_options.ContextLoaderOptions.AdditionalRuntimePaths.Contains(assemblyDirectory) == false)
            {
                _options.ContextLoaderOptions.AdditionalRuntimePaths.Add(assemblyDirectory);
            }

            var dlls = Directory.GetFiles(assemblyDirectory, "*.dll");
            paths.AddRange(dlls);
        }
    }
}
