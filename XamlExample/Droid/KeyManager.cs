using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace RemoteX.Sketch.XamExapmple.Droid
{
    public class KeyManager : IKeyManager
    {
        public event EventHandler<KeyEventArgs> VolumnUpKeyDown;
        public event EventHandler<KeyEventArgs> VolumnUpKeyUp;
        public event EventHandler<KeyEventArgs> VolumnDownKeyDown;
        public event EventHandler<KeyEventArgs> VolumnDownKeyUp;

        internal bool OnActivityKeyUp(Keycode keycode)
        {
            KeyEventArgs keyEventArgs = new KeyEventArgs()
            {
                Handle = false
            };
            if(keycode == Keycode.VolumeDown)
            {
                VolumnDownKeyUp?.Invoke(this, keyEventArgs);
                Log.Info("FUCK", "VolumnDownUp" );
            }
            if(keycode == Keycode.VolumeUp)
            {
                VolumnUpKeyUp?.Invoke(this, keyEventArgs);
            }
            return true;
        }

        internal bool OnActivityKeyDown(Keycode keycode)
        {
            KeyEventArgs keyEventArgs = new KeyEventArgs()
            {
                Handle = false
            };
            if (keycode == Keycode.VolumeDown)
            {
                VolumnDownKeyDown?.Invoke(this, keyEventArgs);
                Log.Info("FUCK", "VolumnDownDown");
            }
            if (keycode == Keycode.VolumeUp)
            {
                VolumnUpKeyDown?.Invoke(this, keyEventArgs);
            }
            return true;
        }
    }
}