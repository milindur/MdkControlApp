using System;
using MDKControl.Core.Factories;
using MDKControl.Core.ViewModels;
using MDKControl.Core.Views;
using Xamarin.Forms;

namespace MDKControl.Core
{
    public class App : Application
    {
        public App(IViewFactory viewFactory)
        {
            viewFactory.Register<DeviceListViewModel, DeviceListView>();
            viewFactory.Register<DeviceViewModel, DeviceView>();

            var mainPage = viewFactory.Resolve<DeviceListViewModel>();
            MainPage = new NavigationPage(mainPage);
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

