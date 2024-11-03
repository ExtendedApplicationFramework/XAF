namespace XAF.Modularity.Catalogs;
public interface IModuleCatalog
{
    Task<Module[]> GetModulesAsync(Func<Type, bool> typeMatch);
}
