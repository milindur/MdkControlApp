using Autofac;
using Autofac.Builder;
using Autofac.Core;
using MDKControl.Services;

namespace MDKControl
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MdkBleDeviceService>();
            builder.RegisterGeneratedFactory<MdkBleDeviceServiceFactory>(new TypedService(typeof(MdkBleDeviceService)));
        }
    }
}
