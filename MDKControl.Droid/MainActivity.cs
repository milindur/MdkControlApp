using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MDKControl.Core;
using Autofac;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Droid
{
    [Activity(Label = "MDK Control", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            var builder = new ContainerBuilder();

            // shared modules
            builder.RegisterModule<CoreModule>();

            // platform-specific registrations
            builder.RegisterInstance(new Ble.Adapter())
                .As<Ble.IAdapter>()
                .SingleInstance();

            var container = builder.Build();

            LoadApplication(container.Resolve<Xamarin.Forms.Application>());
        }
    }
}
