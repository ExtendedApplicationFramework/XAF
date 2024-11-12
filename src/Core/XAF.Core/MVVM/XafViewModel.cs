namespace XAF.Core.MVVM;
public abstract class XafViewModel : NotifyPropertyChanged, IXafViewModel
{
    public BindableTask? LoadTask { get; set; }
    public BindableTask? UnloadTask { get; set; }

    public virtual void Initialize() { }

    public virtual int CompareTo(IXafViewModel? other)
    {
        return 0;
    }

    public virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task UnloadAsync()
    {
        return Task.CompletedTask;
    }
}

public abstract class XafViewModel<TParameter> : XafViewModel, IXafViewModel<TParameter>
{
    public abstract void Initialize(TParameter parameter);
}
