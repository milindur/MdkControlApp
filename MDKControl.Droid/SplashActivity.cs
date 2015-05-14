
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using System.Threading.Tasks;

namespace MDKControl.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Splash);

            await Task.Delay(2000);

            StartActivity(typeof(MainActivity));
        }
    }
}

