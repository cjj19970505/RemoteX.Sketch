using RemoteX.Bluetooth.Rfcomm;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.Bluetooth
{
    public class MouseServiceWrapper
    {
        public Guid Uuid
        {
            get
            {
                return Constants.MouseServiceId;
            }
        }

        public IRfcommConnection RfcommConnection { get; }

        public MouseServiceWrapper(IRfcommConnection connection)
        {
            RfcommConnection = connection;
        }
    }
}
