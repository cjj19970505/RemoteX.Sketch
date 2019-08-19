using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Input;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RemoteX.Bluetooth;
using RemoteX.Bluetooth.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Sketch.XamExapmple.Droid.ManagerManagerFragment))]
namespace RemoteX.Sketch.XamExapmple.Droid
{
    public class ManagerManagerFragment:Fragment, IManagerManager
    {
        const string TAG = "ManagerManagerFragment";

        public RemoteX.Input.Droid.InputManager _InputManager;

        public RemoteX.Input.IInputManager InputManager
        {
            get
            {
                if(_InputManager == null)
                {
                    _InputManager = new RemoteX.Input.Droid.InputManager();
                }
                return _InputManager;
            }
        }
        private IBluetoothManager _BluetoothManager;
        public IBluetoothManager BluetoothManager
        {
            get
            {
                if(_BluetoothManager == null)
                {
                    _BluetoothManager = new BluetoothManager();
                }
                return _BluetoothManager;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            Log.Info(TAG, "ON_START");
        }

        

    }
}