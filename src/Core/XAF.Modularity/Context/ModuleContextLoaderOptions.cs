using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace XAF.Modularity.Context;
public class ModuleContextLoaderOptions
{
    public ResolveAssembliesFromHost ResolveAssembliesFromHost { get; set; } = ResolveAssembliesFromHost.Always;

    //public List<AssemblyName> HostApplicationAssemblies { get; set; } = [];

    public List<AssemblyInfo> AdditionalAssemblies { get; set; } = Default.AdditionalAssemblies;

    public List<string> AdditionalRuntimePaths { get; set; } = Default.AdditionalRuntimePaths;

    public Func<ILogger<ModuleContextLoader>> LoggerFactory { get; set; } = Default.LoggerFactory;

    public static class Default
    {
        public static ResolveAssembliesFromHost ResolveAssembliesFromHost { get; set; } = ResolveAssembliesFromHost.Always;

        //public List<AssemblyName> HostApplicationAssemblies { get; set; } = [];

        public static List<AssemblyInfo> AdditionalAssemblies { get; set; } = [];

        public static List<string> AdditionalRuntimePaths { get; set; } = [];

        public static Func<ILogger<ModuleContextLoader>> LoggerFactory { get; set; } = () => new NullLogger<ModuleContextLoader>();
    }

}
