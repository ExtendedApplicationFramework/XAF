﻿using XAF.UI.Abstraction;

namespace XAF.UI.WPF.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ViewForAttribute<TViewModel> : Attribute
    where TViewModel : IViewModel
{
    public Type ViewModelType { get; }

    public ViewForAttribute()
    {
        ViewModelType = typeof(TViewModel);
    }
}
