using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Foundation;
using GalaSoft.MvvmLight.Views;
using MDKControl.Core;
using MDKControl.Core.Helpers;
using MDKControl.Core.ViewModels;
using MDKControl.iOS.Helpers;
using Microsoft.Practices.ServiceLocation;
using UIKit;
using Xamarin;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private static IContainer _container;

        public const string ModeSmsViewKey = "ModeSmsViewKey";
        public const string ModeSmsStatusViewKey = "ModeSmsStatusViewKey";
        public const string ModePanoViewKey = "ModePanoViewKey";
        public const string ModePanoStatusViewKey = "ModePanoStatusViewKey";
        public const string ModeAstroViewKey = "ModeAstroViewKey";
        public const string ModeAstroStatusViewKey = "ModeAstroStatusViewKey";
        public const string JoystickViewKey = "JoystickViewKey";

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomain_CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Bootstrap();
            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;

            ServiceLocator.Current.GetInstance<DeviceListViewModel>().Cleanup();
        }

        private void Bootstrap()
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

                builder.RegisterInstance(Ble.Adapter.Current)
                    .As<Ble.IAdapter>();
                
                var nav = new NavigationService();
                nav.Initialize((UINavigationController)Window.RootViewController);
                nav.Configure(ViewModelLocator.DeviceListViewKey, "DeviceListViewController");
                nav.Configure(ViewModelLocator.DeviceViewKey, "DeviceViewController");
                nav.Configure(ModeSmsViewKey, "ModeSmsViewController");
                nav.Configure(ModeSmsStatusViewKey, "ModeSmsStatusViewController");
                nav.Configure(ModePanoViewKey, "ModePanoViewController");
                nav.Configure(ModePanoStatusViewKey, "ModePanoStatusViewController");
                nav.Configure(ModeAstroViewKey, "ModeAstroViewController");
                nav.Configure(ModeAstroStatusViewKey, "ModeAstroStatusViewController");
                nav.Configure(JoystickViewKey, "JoystickViewController");

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

        private static void AppDomain_CurrentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is TimeoutException)
            {
                // ignore (for the moment, should communicate to user that there is a communication issue)
                Insights.Report(e.ExceptionObject as Exception);
            }
            else
            {
                Insights.Report(e.ExceptionObject as Exception, Insights.Severity.Error);
            }
        }

        private static void TaskScheduler_UnobservedTaskException (object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception.InnerException is TimeoutException)
            {
                // ignore (for the moment, should communicate to user that there is a communication issue)
                Insights.Report(e.Exception);
                e.SetObserved();
            }
            else
            {
                Insights.Report(e.Exception, Insights.Severity.Error);
            }
        }
    }
}
