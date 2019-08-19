using RemoteX.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Sketch.UwpExample
{
    public class BleDeviceViewModel
    {
        public IBluetoothDevice BluetoothDevice { get; }
        public string Name
        {
            get
            {
                return BluetoothDevice.Name;
            }
        }
        public UInt64 Address
        {
            get
            {
                return BluetoothDevice.Address;
            }
        }
        public BleDeviceViewModel(IBluetoothDevice device)
        {
            BluetoothDevice = device;
        }
    }
}
