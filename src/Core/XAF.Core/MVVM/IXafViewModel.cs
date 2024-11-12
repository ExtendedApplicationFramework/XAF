namespace XAF.Core.MVVM;
public interface IXafViewModel : IComparable<IXafViewModel>
{
    BindableTask? LoadTask { get; set; }

    BindableTask? UnloadTask { get; set; }

    void Initialize();

    Task LoadAsync();

    Task UnloadAsync();
}

public interface IXafViewModel<in TParameter> : IXafViewModel
{
    void Initialize(TParameter parameter);
}