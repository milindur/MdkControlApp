using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using MDKControl.Core;
using Xamarin.Forms;
using Autofac;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();
            
            // create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            // If you have defined a view, add it here:
            // window.RootViewController  = navigationController;

            // make the window visible
            //window.MakeKeyAndVisible();

            var builder = new ContainerBuilder();

            // shared modules
            builder.RegisterModule<CoreModule>();

            // platform-specific registrations
            builder.RegisterInstance(Ble.Adapter.Current)
                .As<Ble.IAdapter>()
                .SingleInstance();

            var container = builder.Build();

            LoadApplication(container.Resolve<Xamarin.Forms.Application>());

            return base.FinishedLaunching(app, options);
        }
    }
}