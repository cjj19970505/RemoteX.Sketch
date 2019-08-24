using RemoteX.Bluetooth.Rfcomm;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Sketch.Bluetooth
{
    public class MouseServiceWrapper
    {
        public event EventHandler<Vector2> OnMouseMoveReceived;
        public static Guid ServiceId
        {
            get
            {
                return Constants.MouseServiceId;
            }
        }
        public RfcommFixedLengthConnectionHandler ConnectionHandler { get; }
        public IRfcommConnection RfcommConnection { get; }
        public Vector2 LatestReading { get; private set; }

        public byte[] EncodedLatestReading
        {
            get
            {
                var xBytes = BitConverter.GetBytes(LatestReading.X);
                var yBytes = BitConverter.GetBytes(LatestReading.Y);
                var byteList = new List<byte>();
                byteList.AddRange(xBytes);
                byteList.AddRange(yBytes);
                return byteList.ToArray();
            }
        }

        public MouseServiceWrapper(IRfcommConnection connection)
        {
            RfcommConnection = connection;
            ConnectionHandler = new RfcommFixedLengthConnectionHandler(RfcommConnection, EncodedLatestReading.Length);
            ConnectionHandler.OnReceived += ConnectionHandler_OnReceived;

        }

        private void ConnectionHandler_OnReceived(object sender, byte[] e)
        {
            var x = BitConverter.ToSingle(e, 0);
            var y = BitConverter.ToSingle(e, sizeof(float));
            var reading = new Vector2(x, y);
            OnMouseMoveReceived?.Invoke(this, reading);
        }

        public async Task MoveMouseAsync(Vector2 moveMount)
        {
            LatestReading = moveMount;
            await ConnectionHandler.SendAsync(EncodedLatestReading);
        }
    }
}
