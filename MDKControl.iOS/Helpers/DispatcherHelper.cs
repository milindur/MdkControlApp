using System;
using MDKControl.Core.Helpers;
using Foundation;

namespace MDKControl.iOS.Helpers
{
    public class DispatcherHelper : IDispatcherHelper
    {
        private NSObject _owner;

        public void SetOwner(NSObject owner)
        {
            _owner = owner;
        }

        public void RunOnUIThread(Action action)
        {
            if (_owner == null)
                throw new InvalidOperationException();
            
            _owner.InvokeOnMainThread(action);
        }
    }
}
