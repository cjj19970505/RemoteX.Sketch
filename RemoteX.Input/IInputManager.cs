using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Input
{
    public interface IInputManager
    {
        
        event EventHandler<IPointer> PointerEntered;
        event EventHandler<IPointer> PointerPressed;
        event EventHandler<IPointer> PointerMoved;
        event EventHandler<IPointer> PointerReleased;
        event EventHandler<IPointer> PointerExited;

    }
}
