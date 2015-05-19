using Autofac;
using Autofac.Builder;
using Autofac.Core;
using MDKControl.Core.Factories;
using MDKControl.Core.Services;
using MDKControl.Core.ViewModels;
using MDKControl.Core.Views;
using Xamarin.Forms;

namespace MDKControl.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<App>()
                .AsSelf()
                .As<Application>();

            builder.RegisterType<ViewFactory>()
                .As<IViewFactory>()
                .SingleInstance();

            builder.RegisterType<Navigator>()
                .As<INavigator>()
                .SingleInstance();

            builder.Register(context => Application.Current.MainPage.Navigation)
                .SingleInstance();

            builder.RegisterType<BleMoCoBusCommService>();

            builder.RegisterType<DeviceListViewModel>();
            builder.RegisterType<DeviceListView>();

            builder.RegisterType<DeviceViewModel>();
            builder.RegisterType<DeviceView>();
        }
    }
}
