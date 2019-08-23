using RemoteX.Bluetooth.LE.Gatt.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.Bluetooth
{
    public class KeyboardServiceClientWrapper
    {
        public IGattClientCharacteristic GattCharacteristic { get; }
        public event EventHandler<KeyStatusChangeEventArgs> OnKeyStatusChanged;
        
        public KeyboardServiceClientWrapper(IGattClientCharacteristic characteristic)
        {
            GattCharacteristic = characteristic;
            GattCharacteristic.OnNotified += GattCharacteristic_OnNotified;
        }

        private void GattCharacteristic_OnNotified(object sender, byte[] e)
        {
            var status = (KeyStatus)e[0];
            var keyCode = (VirtualKeyCode)e[1];
            OnKeyStatusChanged?.Invoke(this, new KeyStatusChangeEventArgs(keyCode, status));
        }

        public class KeyStatusChangeEventArgs : EventArgs
        {
            public VirtualKeyCode KeyCode { get; }
            public KeyStatus NewStatus { get; }
            public KeyStatusChangeEventArgs(VirtualKeyCode keyCode, KeyStatus status):base()
            {
                KeyCode = keyCode;
                NewStatus = status;
            }
        }
    }
}
