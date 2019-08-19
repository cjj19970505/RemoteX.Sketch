using RemoteX.Bluetooth;
using RemoteX.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.XamExapmple
{
    public interface IManagerManager
    {
        IBluetoothManager BluetoothManager { get; }
        IInputManager InputManager { get; }
    }
}
