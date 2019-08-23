using RemoteX.Bluetooth;
using RemoteX.Bluetooth.LE.Gatt;
using RemoteX.Bluetooth.LE.Gatt.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.Bluetooth
{
    public enum KeyStatus { Released = 0, Pressed = 1}
    public class KeyboardServiceWrapper
    {
        public static Guid Guid
        {
            get
            {
                return Constants.KeyboardServiceGuid;
            }
        }
        public IGattServerService GattServerService { get; }
        public IBluetoothManager BluetoothManager { get; }
        public KeyActionCharacteristicWrapper KeyActionCharacteristicWrapper { get; }
        public KeyboardServiceWrapper(IBluetoothManager bluetoothManager)
        {
            BluetoothManager = bluetoothManager;
            KeyActionCharacteristicWrapper = new KeyActionCharacteristicWrapper(this);
            GattServerService = BluetoothManager.NewGattServiceBuilder()
                .SetUuid(Guid)
                .AddCharacteristics(KeyActionCharacteristicWrapper.GattServerCharacteristic)
                .Build();

        }

        public void UpdateKeyStatus(byte keyCode, KeyStatus status)
        {
            KeyActionCharacteristicWrapper.NotifyKeyStatus(keyCode, status);
        }
    }

    public class KeyActionCharacteristicWrapper
    {
        public static Guid Guid
        {
            get
            {
                return Constants.KeyActionCharacteristicWrapper;
            }
        }
        public ClientCharacteristicConfigurationDescriptorWrapper ClientCharacteristicConfigurationDescriptorWrapper { get; }
        public readonly static GattCharacteristicProperties Properties = new GattCharacteristicProperties
        {
            Notify = true
        };
        public readonly static GattPermissions Permissions = new GattPermissions
        {
        };
        public IGattServerCharacteristic GattServerCharacteristic { get; }
        public KeyboardServiceWrapper KeyboardServiceWrapper { get; }

        public void NotifyKeyStatus(byte keyCode, KeyStatus status)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)status;
            bytes[1] = keyCode;
            GattServerCharacteristic.Value = bytes;
            NotifyAll();
        }
        
        public KeyActionCharacteristicWrapper(KeyboardServiceWrapper serviceWrapper)
        {
            KeyboardServiceWrapper = serviceWrapper;
            ClientCharacteristicConfigurationDescriptorWrapper = new ClientCharacteristicConfigurationDescriptorWrapper(KeyboardServiceWrapper.BluetoothManager);
            GattServerCharacteristic = KeyboardServiceWrapper.BluetoothManager.NewGattCharacteristicBuilder()
                .SetUuid(Guid)
                .AddDescriptors(ClientCharacteristicConfigurationDescriptorWrapper.GattServerDescriptor)
                .SetPermissions(Permissions)
                .SetProperties(Properties)
                .Build();
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
