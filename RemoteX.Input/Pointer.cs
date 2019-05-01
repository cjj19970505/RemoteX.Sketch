using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.GraphicMath;

namespace RemoteX.Input
{
    public enum PointerDeviceType { Touch, Pen, Mouse};
    public enum PointerState { Entered, Released, Pressed, Exited};
    public interface IPointer
    {
        
        PointerDeviceType PointerDeviceType { get; }
        PointerState LatestState { get; }
        Vector2 PointerPoint { get; }   
    }
}
