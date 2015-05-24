using System.Threading.Tasks;
using Android.App;
using Android.OS;
using MDKControl.Core;
using Android.Content;
using Microsoft.Practices.ServiceLocation;
using MDKControl.Droid.Helpers;
using MDKControl.Core.ViewModels;

namespace MDKControl.Droid
{
    [Activity(Label = "MDK Control", Icon = "@drawable/ic_launcher", Theme = "@style/Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : ActivityBaseEx
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Splash);

            await Task.Delay(500);

            App.Bootstrap();

            StartActivity(typeof(DeviceListViewActivity));
        }
    }
}

