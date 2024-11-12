using DynamicData;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using XAF.Core.ExtensionMethods;
using XAF.Core.MVVM;

namespace XAF.WPF.UI.Internal;
internal class ViewModelPresenter : IViewModelPresenter
{
    private List<FrameworkElement> _containers;
    private ConcurrentDictionary<FrameworkElement, IDisposable> _disposablesForAdapters;

    private readonly IViewAdapterLocator _viewAdapterLocator;
    private readonly IViewLocator _viewLocator;
    private readonly ILogger<ViewModelPresenter> _logger;
    private readonly CompositeDisposable _compositeDisposable;

    public object Key { get; }
    public IViewCollection SelectedViews { get; }
    public IViewCollection Views { get; }

    public ViewModelPresenter(
        object key,
        IViewAdapterLocator viewAdapterLocator,
        IViewLocator viewLocator,
        ILogger<ViewModelPresenter> logger)
    {
        Key = key;
        _viewAdapterLocator = viewAdapterLocator;
        _viewLocator = viewLocator;
        _logger = logger;

        _containers = [];
        _disposablesForAdapters = [];
        _compositeDisposable = new();

        Views = new ViewCollection();
        SelectedViews = new ViewCollection();
    }

    public void AttachTo(FrameworkElement container)
    {
        if (!_viewAdapterLocator.TryGetAdapterFor(container, out var adapter))
        {
            _logger.LogError("No ViewAdapter for {container} found", container);
            return;
        }

        var disposable = adapter.Attach(container, this);

        if (!_disposablesForAdapters.TryAdd(container, disposable))
        {
            _logger.LogError($"adapter disposable for {container} could not be added");
            disposable.Dispose();
            return;
        }

        _containers.Add(container);
    }

    public void DetachFrom(FrameworkElement container)
    {
        if (!_containers.Remove(container))
        {
            return;
        }

        if (_disposablesForAdapters.TryRemove(container, out var disposable))
        {
            disposable.Dispose();
        }
    }

    public bool Add<TViewModel>(TViewModel viewModel, CancellationToken cancellationToken) where TViewModel : IXafViewModel
    {
        if (Views.ContainsKey(viewModel))
        {
            return false;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var view = _viewLocator.GetViewFor<TViewModel>();
        ViewModelLocator.AttachToView(view, viewModel);

        if (cancellationToken.IsCancellationRequested)
        {
            view = null;
            return false;
        }

        Views.Add(viewModel, view);
        return true;
    }

    public bool Select<TViewModel>(TViewModel vm, CancellationToken cancellation) where TViewModel : IXafViewModel
    {
        if (SelectedViews.ContainsKey(vm))
        {
            return true;
        }

        if (!Views.TryGetValue(vm, out var view))
        {
            return false;
        }

        SelectedViews.Add(vm, view);

        return true;

    }

    public bool Unselect<TViewModel>(TViewModel vm, CancellationToken cancellation) where TViewModel : IXafViewModel
    {
        return SelectedViews.Remove(vm);
    }

    public bool Remove<TViewModel>(TViewModel viewModel, CancellationToken cancellation) where TViewModel : IXafViewModel
    {
        if (!Views.ContainsKey(viewModel))
        {
            return true;
        }

        if (cancellation.IsCancellationRequested)
        {
            return false;
        }

        SelectedViews.Remove(viewModel);

        var view = Views[viewModel];

        ViewModelLocator.DetachFromView(view, viewModel);
        Views.Remove(viewModel);

        return true;
    }
}
