using System.Windows;
using XAF.Core.MVVM;

namespace XAF.WPF.UI;
public interface IViewModelPresenter
{
    object Key { get; }

    IViewCollection SelectedViews { get; }

    IViewCollection Views { get; }

    void AttachTo(FrameworkElement container);

    void DetachFrom(FrameworkElement container);

    bool Add<TViewModel>(TViewModel viewModel, CancellationToken cancellation)
        where TViewModel : IXafViewModel;

    bool Select<TViewModel>(TViewModel vm, CancellationToken cancellation)
        where TViewModel : IXafViewModel;

    bool Unselect<TViewModel>(TViewModel vm, CancellationToken cancellation)
        where TViewModel : IXafViewModel;

    bool Remove<TViewModel>(TViewModel viewModel, CancellationToken cancellation)
        where TViewModel : IXafViewModel;
}
