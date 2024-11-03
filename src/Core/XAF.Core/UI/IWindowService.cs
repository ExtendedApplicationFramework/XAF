using XAF.Core.MVVM;

namespace XAF.Core.UI;
public interface IWindowService
{
    IObservable<DialogParameters> DialogRequested();

    IObservable<DialogParameters> DialogOpened();

    IObservable<DialogParameters> DialogClosed();

    Task<bool> OpenWindowAsync<TViewModel>(CancellationToken cancle)
    where TViewModel : IXafViewModel;

    Task<bool> OpenWindowAsync<TViewModel>(TViewModel vm, CancellationToken cancle)
        where TViewModel : IXafViewModel;

    Task<bool> OpenWindowAsync<TViewModel, TParameter>(TParameter parameter, CancellationToken cancle)
        where TViewModel : class, IXafViewModel<TParameter>;

    Task<bool> OpenWindowAsync<TViewModel, TParameter>(TViewModel vm, TParameter parameter, CancellationToken cancle)
        where TViewModel : IXafViewModel<TParameter>;

    Task<bool> CloseWindowAsync<TViewModel>(TViewModel vm, CancellationToken cancle1)
        where TViewModel : IXafViewModel;

    Task<TResult?> ShowDialogAsync<TResult, TViewModel>()
        where TViewModel : class, IXafDialogViewModel<TResult>;

    Task<TResult?> ShowDialogAsync<TResult, TViewModel>(TViewModel vm)
        where TViewModel : class, IXafDialogViewModel<TResult>;

    Task<TResult?> ShowDialogAsync<TResult, TViewModel, TParameter>(TParameter parameter)
        where TViewModel : class, IXafDialogViewModel<TResult, TParameter>;

    Task<TResult?> ShowDialogAsync<TResult, TViewModel, TParameter>(TViewModel vm, TParameter parameter)
        where TViewModel : class, IXafDialogViewModel<TResult, TParameter>;
}

public record DialogParameters(IXafViewModel ViewModel, object? Parameter, object? Result)
{
    public bool Cancle { get; set; }
}
