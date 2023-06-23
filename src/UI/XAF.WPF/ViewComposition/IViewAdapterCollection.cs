﻿namespace XAF.UI.WPF.ViewComposition;
public interface IViewAdapterCollection
{
    void AddAdapter(IViewAdapter viewAdapter);
    IViewAdapter GetAdapterFor(Type viewType);
}
