using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAF.Core.MVVM;
public interface IXafDialogViewModel<TResult> : IXafViewModel
{
    TResult DefaultResult { get; }

    event EventHandler<TResult>? CloseDialogRequested;

    bool CanClose(TResult result);

    bool Cancle();
}

public interface IXafDialogViewModel<TResult, TParameter> : IXafViewModel<TParameter>, IXafDialogViewModel<TResult>
{

}
