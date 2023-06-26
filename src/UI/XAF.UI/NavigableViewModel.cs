﻿using XAF.UI.Abstraction;

namespace XAF.UI;
public abstract class NavigableViewModel : ViewModelBase, INavigableViewModel
{
    public virtual void OnNavigatedFrom()
    {

    }

    public virtual void OnNavigatedTo()
    {

    }
}

public abstract class NavigableViewModel<T> : NavigableViewModel, INavigableViewModel<T>
{
    public virtual void OnNavigatedTo(T parameter)
    {

    }
}
