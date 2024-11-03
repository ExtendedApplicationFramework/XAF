using XAF.Core.MVVM;

namespace XAF.Core.UI;
public interface IViewCompositionService
{
    Task<bool> AddViewAsync<TViewModel>(object presenterKey, CancellationToken cancle)
        where TViewModel : class, IXafViewModel;

    Task<bool> AddViewAsync<TViewModel, TParameter>(TParameter parameter, object presenterKey, CancellationToken cancle)
        where TViewModel : class, IXafViewModel<TParameter>;

    Task<bool> AddViewAsync<TViewModel>(TViewModel vm, object presenterKey, CancellationToken cancle)
        where TViewModel : IXafViewModel;

    Task<bool> AddViewAsync<TViewModel, TParameter>(TViewModel vm, TParameter parameter, object presenterKey, CancellationToken cancle)
        where TViewModel : IXafViewModel<TParameter>;

    Task<bool> SelectViewAsync<TViewModel>(TViewModel vm, object presenterKey, CancellationToken cancle)
        where TViewModel : IXafViewModel;

    Task<bool> SelectViewAsync<TViewModel, TParameter>(TViewModel vm, TParameter parameter, object presenterKey, CancellationToken cancle)
        where TViewModel : IXafViewModel<TParameter>;

    Task<bool> RemoveViewAsync<TViewModel>(TViewModel vm, object presenterKey, CancellationToken cancle)
        where TViewModel : IXafViewModel;

    IObservable<ViewManipulation> ViewManipulationRequested();
    IObservable<ViewManipulation> ViewManipulationRequested(object presenterKey);

    IObservable<ViewManipulation> ViewManipulationCompleted();
    IObservable<ViewManipulation> ViewManipulationCompleted(object presenterKey);
}

public record ViewManipulation(ViewManipulationType Type, IXafViewModel ViewModel, object PresenterKey, object? Parameter = null)
{
    public bool Cancle { get; set; }
}
