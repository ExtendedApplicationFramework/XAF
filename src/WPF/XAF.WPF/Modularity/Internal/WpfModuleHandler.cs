using XAF.Core.UI;
using XAF.Modularity;
using XAF.WPF.UI;

namespace XAF.WPF.Modularity.Internal;
internal class WpfModuleHandler : ModuleHandler<IWpfModule>
{
    private readonly IViewLocator _viewLocator;
    private readonly IViewCompositionService _viewCompositionService;

    public WpfModuleHandler(IViewLocator viewLocator, IViewCompositionService viewCompositionService)
    {
        _viewLocator = viewLocator;
        _viewCompositionService = viewCompositionService;
    }

    public override Task LoadAsync(IWpfModule module)
    {
        return _viewLocator.DiscoverViewsAsync(module.GetType().Assembly);
    }

    public override Task StartAsync(IWpfModule module)
    {
        return module.StartAsync(_viewCompositionService);
    }
}
