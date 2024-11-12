using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XAF.Core.MVVM;

namespace XAF.Core;
public interface IViewModelLocator
{
    IXafViewModel CreateVm(Type vmType);

    TViewModel CreateVm<TViewModel>()
        where TViewModel : IXafViewModel;

    void Prepare<TViewModel>(TViewModel vm, FrameworkElement view)
        where TViewModel : IXafViewModel;

    void Prepare<TViewModel, TParameter>(TViewModel vm, TParameter parameter, FrameworkElement view)
        where TViewModel : IXafViewModel<TParameter>;
}
