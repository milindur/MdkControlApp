using Autofac;
using Autofac.Builder;
using Autofac.Core;
using MDKControl.Core.Services;
using MDKControl.Core.ViewModels;
using Xamarin.Forms;
using MDKControl.Core.Views;

namespace MDKControl.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<App>()
                .AsSelf()
                .As<Application>();

            builder.RegisterType<BleMoCoBusDeviceService>();
            builder.RegisterGeneratedFactory<BleMoCoBusDeviceServiceFactory>(new TypedService(typeof(BleMoCoBusDeviceService)));

            builder.RegisterType<DeviceListViewModel>();
            builder.RegisterType<DeviceListView>();
        }
    }
}
