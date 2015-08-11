using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MDKControl.Core;
using MDKControl.Core.ViewModels;
using MDKControl.Droid.Helpers;
using Microsoft.Practices.ServiceLocation;
using Xamarin;
using Android.Runtime;

namespace MDKControl.Droid.Activities
{
    [Activity(Label = "MDK Control", Icon = "@drawable/ic_launcher", Theme = "@style/Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : ActivityBaseEx
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            /*AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                args.Handled = true;
            };*/

            Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
            {
                if (isStartupCrash) {
                    Insights.PurgePendingCrashReports().Wait();
                }
            };
#if DEBUG 
            Insights.Initialize(Insights.DebugModeKey, this);
#else
            Insights.Initialize("76d51a30602c5fd9a5e64f263e25d14947533c61", this);
#endif
            Insights.Track("Startup");

            SetContentView(Resource.Layout.Splash);

            await Task.Delay(400);

            App.Bootstrap();

            ServiceLocator.Current.GetInstance<DispatcherHelper>().SetOwner(this);

            Insights.Track("StartDeviceListViewActivity");
            StartActivity(typeof(DeviceListViewActivity));
        }
    }
}

