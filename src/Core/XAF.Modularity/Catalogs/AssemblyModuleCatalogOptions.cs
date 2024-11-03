using XAF.Modularity.Context;

namespace XAF.Modularity.Catalogs;

public class AssemblyModuleCatalogOptions
{
    public ModuleContextLoaderOptions ContextLoaderOptions { get; set; } = Default.ContextLoaderOptions;

    public static class Default
    {
        public static ModuleContextLoaderOptions ContextLoaderOptions { get; set; } = new();
    }
}