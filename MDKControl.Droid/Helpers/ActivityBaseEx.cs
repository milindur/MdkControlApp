using System;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace MDKControl.Droid.Helpers
{
    public class ActivityBaseEx : ActivityBase
    {
        private static int _activeCount;
        
        public IDialogService Dialog => ServiceLocator.Current.GetInstance<IDialogService>();

        public NavigationService GlobalNavigation => (NavigationService)ServiceLocator.Current.GetInstance<INavigationService>();

        protected override void OnStart()
        {
            base.OnStart();

            if (_activeCount == 0)
                OnApplicationStart();
            _activeCount++;
        }

        protected override void OnStop()
        {
            _activeCount--;
            if (_activeCount == 0)
                OnApplicationStop();

            base.OnStop();
        }

        protected virtual void OnApplicationStart()
        {
            Console.WriteLine("Application: Start");
        }

        protected virtual void OnApplicationStop()
        {
            Console.WriteLine("Application: Stop");
        }
    }
}
