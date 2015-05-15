using System;
using MDKControl.Core.ViewModels;
using MDKControl.Core.Views;
using Xamarin.Forms;

namespace MDKControl.Core
{
    public class App : Application
    {
        public App(DeviceListViewModel deviceListViewModel, Func<DeviceListViewModel, DeviceListView> deviceListViewFactory)
        {
            MainPage = new NavigationPage(deviceListViewFactory(deviceListViewModel));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

