using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch
{
    public interface IInputComponent
    {
        int Level { get; }
        IArea StartRegion { get; }
    }
}
