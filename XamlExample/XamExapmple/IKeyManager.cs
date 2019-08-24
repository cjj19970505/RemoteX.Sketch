using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.XamExapmple
{
    public interface IKeyManager
    {
        event EventHandler<KeyEventArgs> VolumnUpKeyDown;
        event EventHandler<KeyEventArgs> VolumnUpKeyUp;
        event EventHandler<KeyEventArgs> VolumnDownKeyDown;
        event EventHandler<KeyEventArgs> VolumnDownKeyUp;
    }

    public class KeyEventArgs:EventArgs
    {
        public bool Handle { get; set; }

    }
}
