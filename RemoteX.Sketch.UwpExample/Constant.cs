using RemoteX.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Sketch.UwpExample
{
    class Constant
    {
        public readonly static Guid GyroscopeServiceGuid = BluetoothUtils.ShortValueUuid(0x3233);
        public readonly static Guid GyroscopeAngularVelocityCharacteristicGuid = BluetoothUtils.ShortValueUuid(0x8923);
        public readonly static Guid ClientRfcommServiceGuid = BluetoothUtils.ShortValueUuid(0x7777);
        public readonly static Guid ClientRfcommMacAddressCharacteristicGuid = BluetoothUtils.ShortValueUuid(0x8888);
        public readonly static Guid RfcommGyroServiceId = Guid.Parse("f27267a3-af7c-4744-934c-ff51b8c3e95d");
    }
}
