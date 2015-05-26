using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core;
using MDKControl.Core.Helpers;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using Microsoft.Practices.ServiceLocation;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Droid
{
    public static class App
    {
        private static IContainer container;
        
        public static void Bootstrap()
        {
            var builder = new ContainerBuilder();

            // shared modules

            builder.RegisterModule<CoreModule>();

            // platform-specific registrations

            builder.RegisterType<DispatcherHelper>()
                .AsSelf()
                .As<IDispatcherHelper>()
                .SingleInstance();

            builder.RegisterInstance(new Ble.Adapter())
                .As<Ble.IAdapter>()
                .SingleInstance();

            var nav = new NavigationService();
            nav.Configure(ViewModelLocator.DeviceListViewKey, typeof(DeviceListViewActivity));
            nav.Configure(ViewModelLocator.DeviceViewKey, typeof(DeviceViewActivity));

            builder.RegisterInstance(nav)
                .As<INavigationService>();

            builder.RegisterType<DialogService>()
                .As<IDialogService>();

            container = builder.Build();

            var csl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => csl);
        }
    }
}
