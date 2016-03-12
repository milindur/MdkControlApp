using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core;
using MDKControl.Core.Helpers;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Activities;
using MDKControl.Droid.Helpers;
using Microsoft.Practices.ServiceLocation;
using Xamarin;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Droid
{
    [Application]
    public class App : Application
    {
        private static IContainer _container;

        public App(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += (sender, e) =>
                {
                    if (e.ExceptionObject is TimeoutException)
                    {
                        // ignore (for the moment, should communicate to user that there is a communication issue)
                    }
                    else
                    {
                        Insights.Report(e.ExceptionObject as Exception, Insights.Severity.Error);
                    }
                };

            Bootstrap();
        }

        private static void Bootstrap()
        {
            System.Diagnostics.Debug.WriteLine("Bootstrap");

            if (_container == null)
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
                    .As<Ble.IAdapter>();

                var nav = new NavigationService();
                nav.Configure(ViewModelLocator.DeviceListViewKey, typeof(DeviceListViewActivity));
                nav.Configure(ViewModelLocator.DeviceViewKey, typeof(DeviceViewActivity));

                builder.RegisterInstance(nav)
                    .As<INavigationService>();

                builder.RegisterType<DialogService>()
                    .As<IDialogService>()
                    .SingleInstance();

                _container = builder.Build();
            }

            if (!ServiceLocator.IsLocationProviderSet)
            {
                var csl = new AutofacServiceLocator(_container);
                ServiceLocator.SetLocatorProvider(() => csl);
            }
        }

        public static void Initialize(Context context)
        {
            if (!Insights.IsInitialized)
            {
                Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
                {
                    if (isStartupCrash)
                    {
                        Insights.PurgePendingCrashReports().Wait();
                    }
                };

#if DEBUG
                Insights.Initialize("1532003910b06b50a709d0e691a8054e904922c8", context);
#else
                Insights.Initialize("76d51a30602c5fd9a5e64f263e25d14947533c61", context);
#endif

                Insights.Track("Initialize");
            }
        }
    }
}
