using Autofac;
using Autofac.Builder;
using Autofac.Core;
using MDKControl.Core.Services;
using MDKControl.Core.ViewModels;

namespace MDKControl.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BleMoCoBusCommService>()
                .As<IMoCoBusCommService>();

            builder.RegisterType<MoCoBusProtocolService>()
                .As<IMoCoBusProtocolService>();

            builder.RegisterType<MoCoBusProtocolMainService>()
                .As<IMoCoBusProtocolMainService>();

            builder.RegisterType<MoCoBusProtocolCameraService>()
                .As<IMoCoBusProtocolCameraService>();

            builder.RegisterType<MoCoBusProtocolMotorService>()
                .As<IMoCoBusProtocolMotorService>();

            builder.RegisterType<DeviceListViewModel>()
                .SingleInstance();
            
            builder.RegisterType<DeviceViewModel>();
        }
    }
}
