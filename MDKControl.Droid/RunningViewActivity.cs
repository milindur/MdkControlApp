using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using MDKControl.Droid.Widgets;
using Microsoft.Practices.ServiceLocation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MDKControl.Droid
{
    [Activity(Label = "Running", ScreenOrientation = ScreenOrientation.Portrait)]
    public class RunningViewActivity : ActivityBaseEx
    {
        public RunningViewActivity()
        {
        }

        public DeviceViewModel Vm { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.RunningView);

            Vm = GlobalNavigation.GetAndRemoveParameter<DeviceViewModel>(Intent);
        }

        protected override void OnResume()
        {
            base.OnResume();

            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }
    }
}
