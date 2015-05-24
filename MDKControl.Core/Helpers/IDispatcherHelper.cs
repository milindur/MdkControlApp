using System;

namespace MDKControl.Core.Helpers
{
    public interface IDispatcherHelper
    {
        void RunOnUIThread(Action action);
    }
}
