using System;

namespace MDKControl.Core
{

    public static class MdkBleConstants
    {
        public static readonly Guid DescriptorClientCharacteristicConfig = new Guid("00002902-0000-1000-8000-00805f9b34fb");

        public static readonly Guid ServiceDeviceInformation = new Guid("0000180a-0000-1000-8000-00805f9b34fb");
        public static readonly Guid ServiceDeviceInformationCharacteristicManufacturer = new Guid("00002a29-0000-1000-8000-00805f9b34fb");

        public static readonly Guid ServiceMoCoBus = new Guid("a3a9eb86-c0fd-4a5c-b191-bff60a7f9ea7");
        public static readonly Guid ServiceMoCoBusCharacteristicRx = new Guid("f897177b-aee8-4767-8ecc-cc694fd5fcee");
        public static readonly Guid ServiceMoCoBusCharacteristicTx = new Guid("bf45e40a-de2a-4bc8-bba0-e5d6065f1b4b");
    }
    
}
