using System.Threading.Tasks;
using Android.App;
using Android.OS;

namespace MDKControl.Droid
{
    [Activity(Label = "MDK Control", Icon = "@drawable/ic_launcher", Theme = "@style/Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Splash);

            await Task.Delay(2000);

            StartActivity(typeof(MainActivity));
        }
    }
}

