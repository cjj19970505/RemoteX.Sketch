using RemoteX.Bluetooth;
using RemoteX.Bluetooth.LE.Gatt;
using RemoteX.Bluetooth.LE.Gatt.Server;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.XamExapmple.BleServices
{
    public class GyroscopeServiceWrapper
    {
        public IGattServerService GattServerService { get; private set; }
        public IBluetoothManager BluetoothManager { get; }
        public Guid Uuid = BluetoothUtils.ShortValueUuid(0x1234);
        GyroscopeAngularVelocityCharacteristicWrapper AngularVelocityCharacteristicWrapper;
        public GyroscopeServiceWrapper(IBluetoothManager bluetoothManager)
        {
            BluetoothManager = bluetoothManager;
            AngularVelocityCharacteristicWrapper = new GyroscopeAngularVelocityCharacteristicWrapper(this);
            IGattServiceBuilder builder = bluetoothManager.NewGattServiceBuilder();
            GattServerService = builder.SetUuid(Uuid)
                .AddCharacteristics(AngularVelocityCharacteristicWrapper.GattServerCharacteristic)
                .Build();

        }

        public void UpdateAngularVelocity(Vector3 value)
        {
            AngularVelocityCharacteristicWrapper.UpdateValue(value);
        }
    }

    public class GyroscopeAngularVelocityCharacteristicWrapper
    {
        public Guid Uuid = BluetoothUtils.ShortValueUuid(0x1235);
        public ClientCharacteristicConfigurationDescriptorWrapper ClientCharacteristicConfigurationDescriptorWrapper { get; }
        public GyroscopeServiceWrapper GyroscopeServiceWrapper { get; }
        private static GattCharacteristicProperties Properties = new GattCharacteristicProperties
        {
            Read = true,
            Notify = true
        };
        private static GattPermissions Permission = new GattPermissions
        {
            Read = true
        };
        public IGattServerCharacteristic GattServerCharacteristic { get; private set; }
        public GyroscopeAngularVelocityCharacteristicWrapper(GyroscopeServiceWrapper serviceWrapper)
        {
            ClientCharacteristicConfigurationDescriptorWrapper = new ClientCharacteristicConfigurationDescriptorWrapper(serviceWrapper.BluetoothManager);
            GyroscopeServiceWrapper = serviceWrapper;
            var builder = GyroscopeServiceWrapper.BluetoothManager.NewGattCharacteristicBuilder();
            GattServerCharacteristic = builder.SetUuid(Uuid)
                .AddDescriptors(ClientCharacteristicConfigurationDescriptorWrapper.GattServerDescriptor)
                .SetPermissions(Permission)
                .SetProperties(Properties)
                .Build();
            GattServerCharacteristic.OnRead += GattServerCharacteristic_OnRead;
        }

        private void GattServerCharacteristic_OnRead(object sender, ICharacteristicReadRequest e)
        {
            e.RespondWithValue(AngularVelocityInBytes);
        }

        public void UpdateValue(Vector3 angularVelocity)
        {
            AngularVelocity = angularVelocity;
            GattServerCharacteristic.Value = AngularVelocityInBytes;
            NotifyAll();
        }

        public Vector3 AngularVelocity { get; private set; }
        public byte[] AngularVelocityInBytes
        {
            get
            {
                var xByte = BitConverter.GetBytes(AngularVelocity.X);
                var yByte = BitConverter.GetBytes(AngularVelocity.Y);
                var zByte = BitConverter.GetBytes(AngularVelocity.Z);
                var byteList = new List<byte>();
                byteList.AddRange(xByte);
                byteList.AddRange(yByte);
                byteList.AddRange(zByte);
                return byteList.ToArray();
            }
        }

        public void NotifyAll()
        {
            var clientConfigurations = ClientCharacteristicConfigurationDescriptorWrapper.ClientConfigurations;
            foreach(var pair in clientConfigurations)
            {
                if(pair.Value.Notifications)
                {
                    GattServerCharacteristic.NotifyValueChanged(pair.Key, false);
                }
            }
        }
    }

}
