using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MDKControl.Core;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using Microsoft.Practices.ServiceLocation;

namespace MDKControl.Droid
{
    [Activity(Label = "MDK Control", Icon = "@drawable/ic_launcher", Theme = "@style/Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : ActivityBaseEx
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Splash);

            await Task.Delay(400);

            App.Bootstrap();

            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);

            StartActivity(typeof(DeviceListViewActivity));
        }
    }
}

