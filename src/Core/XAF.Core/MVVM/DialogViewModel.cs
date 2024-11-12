using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAF.Core.MVVM;
public abstract class DialogViewModel<TResult> : XafViewModel, IXafDialogViewModel<TResult>
{
    public abstract TResult DefaultResult { get; }

    public event EventHandler<TResult>? CloseDialogRequested;

    public virtual bool Cancle()
    {
        if (!CanClose(DefaultResult))
        {
            return false;
        }

        CloseDialog(DefaultResult);

        return true;
    }

    public virtual bool CanClose(TResult result)
    {
        return true;
    }

    protected bool CloseDialog(TResult result)
    {
        if (CanClose(result))
        {
            CloseDialogRequested?.Invoke(this, result);
            return true;
        }
        return false;
    }
}
