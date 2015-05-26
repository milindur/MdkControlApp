using System;
using Android.App;
using MDKControl.Core.Helpers;

namespace MDKControl.Droid.Helpers
{
    public class DispatcherHelper : IDispatcherHelper
    {
        private Activity _owner;

        public void SetOwner(Activity owner)
        {
            _owner = owner;
        }

        public void RunOnUIThread(Action action)
        {
            if (_owner == null)
                throw new InvalidOperationException();
            
            _owner.RunOnUiThread(action);
        }
    }
}
