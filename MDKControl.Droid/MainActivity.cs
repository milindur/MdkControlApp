using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using MDKControl.Core;
using Ble = Robotics.Mobile.Core.Bluetooth.LE;

namespace MDKControl.Droid
{
    [Activity(Label = "MDK Control", Icon = "@drawable/ic_launcher", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

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
