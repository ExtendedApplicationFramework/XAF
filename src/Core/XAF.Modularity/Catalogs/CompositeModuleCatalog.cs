namespace XAF.Modularity.Catalogs;
public class CompositeModuleCatalog : IModuleCatalog
{

    private readonly HashSet<IModuleCatalog> _catalogs = [];

    public async Task<IModuleDescription[]> GetModulesAsync(Func<Type, bool> typeMatch)
    {
        var tasks = _catalogs.Select(c => c.GetModulesAsync(typeMatch));
        var m = await Task.WhenAll(tasks);
        return m.SelectMany(m => m).ToArray();
    }

    public bool Add(IModuleCatalog catalog)
    {
        return _catalogs.Add(catalog);
    }

    public bool Remove(IModuleCatalog catalog)
    {
        return _catalogs.Remove(catalog);
    }
}
