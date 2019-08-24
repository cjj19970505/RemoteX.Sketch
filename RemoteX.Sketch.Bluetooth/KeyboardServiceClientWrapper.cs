using RemoteX.Bluetooth.LE.Gatt.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.Bluetooth
{
    public class KeyboardServiceClientWrapper
    {
        public IGattClientCharacteristic GattKeyActionCharacteristic { get; }
        public IGattClientCharacteristic GattMouseActionCharactristic { get; }
        public event EventHandler<KeyStatusChangeEventArgs> OnKeyStatusChanged;
        public event EventHandler<MouseStatusChangeEventArgs> OnMouseStatusChanged;
        
        public KeyboardServiceClientWrapper(IGattClientCharacteristic keyActionCharacteristic, IGattClientCharacteristic mouseActionCharacteristic)
        {
            GattKeyActionCharacteristic = keyActionCharacteristic;
            GattKeyActionCharacteristic.OnNotified += GattCharacteristic_OnNotified;
            GattMouseActionCharactristic = mouseActionCharacteristic;
            GattMouseActionCharactristic.OnNotified += GattMouseActionCharactristic_OnNotified;
        }

        private void GattMouseActionCharactristic_OnNotified(object sender, byte[] e)
        {
            var mouseButton = e[0];
            var mouseStatus = e[1];
            OnMouseStatusChanged?.Invoke(this, new MouseStatusChangeEventArgs(mouseButton, mouseStatus));
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

        public class MouseStatusChangeEventArgs : EventArgs
        {
            public byte MouseButton { get; }
            public byte NewStatus { get; }
            public MouseStatusChangeEventArgs(byte mouseButton, byte status) : base()
            {
                MouseButton = mouseButton;
                NewStatus = status;
            }
        }
    }
}
