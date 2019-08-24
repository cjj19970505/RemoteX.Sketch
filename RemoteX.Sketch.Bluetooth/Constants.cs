using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Bluetooth;

namespace RemoteX.Sketch.Bluetooth
{
    public static class Constants
    {
        public readonly static Guid GyroscopeRfcommServiceId = Guid.Parse("e8a7ed87-5904-4911-915a-03ab1fa64b41");
        public readonly static Guid KeyboardServiceGuid = BluetoothUtils.ShortValueUuid(0x3450);
        public readonly static Guid KeyActionCharacteristicWrapper = BluetoothUtils.ShortValueUuid(0x3451);
        public readonly static Guid MouseActionCharacteristicWrapper = BluetoothUtils.ShortValueUuid(0x3452);
        public readonly static Guid MouseServiceId = Guid.Parse("0d1ac1b9-528d-43bb-adec-3ce07928d84f");
        

    }
}
