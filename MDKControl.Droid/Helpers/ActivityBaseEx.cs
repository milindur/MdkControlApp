using System;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace MDKControl.Droid.Helpers
{
    public class ActivityBaseEx : ActivityBase
    {
        private static int _activeCount = 0;
        
        public IDialogService Dialog
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDialogService>();
            }
        }

        public NavigationService GlobalNavigation
        {
            get
            {
                return (NavigationService)ServiceLocator.Current.GetInstance<INavigationService>();
            }
        }

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
