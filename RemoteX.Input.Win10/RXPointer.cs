using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RemoteX.Input.Win10
{
    internal class RXPointer : IPointer
    {
        public PointerDeviceType PointerDeviceType
        {
            get
            {
                return Win10PointerPoint.PointerDevice.PointerDeviceType.ToRXDeviceType();
            }
        }

        public PointerState LatestState { get; set; }

        public Vector2 PointerPoint
        {
            get
            {
                Windows.Foundation.Point win10Point = Win10PointerPoint.Position;
                return new Vector2((float)win10Point.X, (float)win10Point.Y);
            }
        }

        public Windows.UI.Input.PointerPoint Win10PointerPoint { get; set; }
        public IInputManager InputManager { get; set; }

        

        public RXPointer(InputManager inputManager)
        {
            InputManager = inputManager;
        }

        public void UpdatePointer(Windows.UI.Input.PointerPoint win10PointerPoint, PointerState latestState)
        {
            Win10PointerPoint = win10PointerPoint;
            LatestState = latestState;
        }

        public override string ToString()
        {
            return PointerDeviceType + " " + PointerPoint;
        }


    }
}
