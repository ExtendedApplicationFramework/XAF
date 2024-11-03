using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace XAF.Modularity.Context;
public class ModuleContextLoader : AssemblyLoadContext
{
    private readonly string _modulePath;
    private readonly AssemblyDependencyResolver _dependencyResolver;
    private readonly ModuleContextLoaderOptions _options;
    protected ILogger<ModuleContextLoader> Logger => _lazyLogger.Value;
    private readonly Lazy<ILogger<ModuleContextLoader>> _lazyLogger;
    private readonly List<AssemblyInfo> _additionalAssemblies;

    public ModuleContextLoader(Assembly assembly, ModuleContextLoaderOptions? options = null, bool enableUnloading = false) : this(assembly.Location, options, enableUnloading)
    {
    }

    public ModuleContextLoader(string modulePath, ModuleContextLoaderOptions? options = null, bool enableUnloading = false) : base(enableUnloading)
    {
        _modulePath = modulePath;
        _dependencyResolver = new AssemblyDependencyResolver(modulePath);
        _options = options ?? new ModuleContextLoaderOptions();
        _additionalAssemblies = _options.AdditionalAssemblies;
        _lazyLogger = new(_options.LoggerFactory());
    }

    public Assembly? Load()
    {
        var name = new AssemblyName(Path.GetFileNameWithoutExtension(_modulePath));

        var result = LoadFromAssemblyName(name);
        return result;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (ShouldLoadFromHostAssemblies(assemblyName))
        {
            if (TryLoadFromHostAssemblyLoadContext(assemblyName, out var result))
            {
                Logger.LogDebug("Assembly {AssemblyName} loaded through host application's AssemblyLoadContext.", assemblyName);
                return null;
            }

            Logger.LogDebug("Host application's AssemblyLoadContext doesn't contain {AssemblyName}. Try to resolve it trough the plugin's references", assemblyName);
        }

        string? assemblyPath;

        var assemblyFileName = assemblyName.Name + ".dll";

        if (_additionalAssemblies.Any(x => assemblyFileName == x.FileName))
        {
            Logger.LogDebug("Found assembly {AssemblyName} in additionalAssemblies", assemblyName);
            assemblyPath = _additionalAssemblies.First(x => assemblyFileName == x.FileName).Path;
        }
        else
        {
            Logger.LogDebug("Try locating Assembly '{AssemblyName}' with default resolver", assemblyName);
            assemblyPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);
        }

        if (assemblyPath != null)
        {
            Logger.LogDebug("Loading {AssemblyName} into AssemblyLoadContext from {Path}", assemblyName, assemblyPath);

            return LoadFromAssemblyPath(assemblyPath);
        }

        if (_options.AdditionalRuntimePaths?.Any() != true)
        {
            Logger.LogWarning("Couldn't locate assembly using {AssemblyName}. Please try adding AdditionalRuntimePaths", assemblyName);
            return null;
        }

        foreach (var runtimePath in _options.AdditionalRuntimePaths)
        {
            var filePath = Directory.GetFiles(runtimePath, assemblyFileName, SearchOption.AllDirectories).FirstOrDefault();
            if (filePath != null)
            {
                Logger.LogDebug("Located {AssemblyName} to {AssemblyPath} using {AdditionalRuntimePath}", assemblyName, filePath, runtimePath);
                return LoadFromAssemblyPath(filePath);
            }
        }

        Logger.LogWarning("Couldn't locate assembly '{AssemblyName}'. Please try adding AdditionalRuntimePaths", assemblyName);

        return null;

    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var nativeHint = _additionalAssemblies.FirstOrDefault(x => x.IsNative && x.FileName == unmanagedDllName);

        if (nativeHint != null)
        {
            return LoadUnmanagedDllFromPath(nativeHint.Path);
        }

        var libraryPath = _dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);

        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }

    private bool TryLoadFromHostAssemblyLoadContext(AssemblyName assemblyName, [NotNullWhenAttribute(true)] out Assembly? assembly)
    {
        try
        {
            assembly = Default.LoadFromAssemblyName(assemblyName);
        }
        catch
        {
            assembly = null;
            return false;
        }

        return true;
    }

    private bool ShouldLoadFromHostAssemblies(AssemblyName assemblyName)
    {
        Logger.LogDebug("Determining if {AssemblyName} should be loaded from host application's AssemblyLoadContext", assemblyName);

        switch (_options.ResolveAssembliesFromHost)
        {
            case ResolveAssembliesFromHost.Never:
                Logger.LogDebug("ResolveAssembliesFromHost is set to {ResolveAssembliesFromHost}. Try to load assembly ({AssemblyName} from plugin's AssemblyLoadContext)", _options.ResolveAssembliesFromHost, assemblyName);
                return false;

            case ResolveAssembliesFromHost.Always:
                Logger.LogDebug("ResolveAssembliesFromHost is set to {ResolveAssembliesFromHost}. Try to load assembly ({AssemblyName} from host application's AssemblyLoadContext)", _options.ResolveAssembliesFromHost, assemblyName);
                return true;

            case ResolveAssembliesFromHost.PreferPlugin:
                Logger.LogDebug("ResolveAssembliesFromHost is set to {ResolveAssembliesFromHost}. Try to load assembly ({AssemblyName} from plugin's AssemblyLoadContext fallback to host application's AssemblyLOadContext if not found)", _options.ResolveAssembliesFromHost, assemblyName);
                return false;

            default:
                return false;
        }
    }
}
