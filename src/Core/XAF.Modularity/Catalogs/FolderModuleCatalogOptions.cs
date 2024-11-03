using XAF.Modularity.Context;

namespace XAF.Modularity.Catalogs;

public class FolderModuleCatalogOptions
{
    public bool IncludeSubFolder { get; set; } = Default.IncludeSubFolder;

    public List<string> SearchPatterns { get; } = Default.SearchPatterns;

    public ModuleContextLoaderOptions ContextLoaderOptions { get; set; } = Default.ContextLoaderOptions;

    public static class Default
    {
        public static bool IncludeSubFolder { get; set; } = true;

        public static List<string> SearchPatterns { get; } = ["*.dll"];

        public static ModuleContextLoaderOptions ContextLoaderOptions { get; set; } = new();
    }
}