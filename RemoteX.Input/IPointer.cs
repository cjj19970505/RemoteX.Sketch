using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace RemoteX.Input
{
    public enum PointerDeviceType { Touch, Pen, Mouse};
    public enum PointerState { Entered, Released, Pressed, Exited};
    public interface IPointer
    {
        IInputManager InputManager { get; }
        PointerDeviceType PointerDeviceType { get; }
        PointerState LatestState { get; }
        Vector2 PointerPoint { get; }   
    }
}
