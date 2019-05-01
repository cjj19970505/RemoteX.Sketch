using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Input.Win10
{
    internal static class Utility
    {
        public static PointerDeviceType ToRXDeviceType(this Windows.Devices.Input.PointerDeviceType self)
        {
            switch(self)
            {
                case Windows.Devices.Input.PointerDeviceType.Mouse:
                    return PointerDeviceType.Mouse;
                case Windows.Devices.Input.PointerDeviceType.Pen:
                    return PointerDeviceType.Pen;
                case Windows.Devices.Input.PointerDeviceType.Touch:
                    return PointerDeviceType.Touch;
                default:
                    return (PointerDeviceType)self;
            }
        }
    }
}
