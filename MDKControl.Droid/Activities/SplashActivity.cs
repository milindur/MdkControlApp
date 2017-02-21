using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MDKControl.Droid.Helpers;
using Microsoft.Practices.ServiceLocation;

namespace MDKControl.Droid.Activities
{
    [Activity(Label = "MDK Control", Icon = "@drawable/ic_launcher", Theme = "@style/Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : ActivityBaseEx
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            System.Diagnostics.Debug.WriteLine("Create SplashActivity");
            SetContentView(Resource.Layout.Splash);

            App.Initialize(this);
            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);

            await Task.Delay(400);

            // FIXME: Insights.Track("StartDeviceListViewActivity");
            StartActivity(typeof(DeviceListViewActivity));
        }
    }
}

