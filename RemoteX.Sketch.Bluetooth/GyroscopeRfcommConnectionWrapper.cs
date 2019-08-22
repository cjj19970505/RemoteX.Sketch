using RemoteX.Bluetooth.Rfcomm;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Sketch.Bluetooth
{
    public class GyroscopeRfcommServiceConnectionWrapper
    {
        public static Guid RfcommServiceId
        {
            get
            {
                return Constants.GyroscopeRfcommServiceId;
            }
        }
        public RfcommFixedLengthConnectionHandler ConnectionHandler { get; }
        public Vector3 LatestReading { get; private set; }
        public event EventHandler<Vector3> OnReadingUpdated;
        TestJitterFixer TestJitterFixer;
        public byte[] EncodedLatestReading
        {
            get
            {
                var xBytes = BitConverter.GetBytes(LatestReading.X);
                var yBytes = BitConverter.GetBytes(LatestReading.Y);
                var zBytes = BitConverter.GetBytes(LatestReading.Z);
                var byteList = new List<byte>();
                byteList.AddRange(xBytes);
                byteList.AddRange(yBytes);
                byteList.AddRange(zBytes);
                return byteList.ToArray();
            }
        }
        public GyroscopeRfcommServiceConnectionWrapper(IRfcommConnection connection)
        {
            LatestReading = Vector3.Zero;
            TestJitterFixer = new TestJitterFixer();
            ConnectionHandler = new RfcommFixedLengthConnectionHandler(connection, EncodedLatestReading.Length);
            ConnectionHandler.OnReceived += ConnectionHandler_OnReceived;
            TestJitterFixer.DataEmited += TestJitterFixer_DataEmited;
        }

        private void TestJitterFixer_DataEmited(object sender, byte[] e)
        {
            var x = BitConverter.ToSingle(e, 0);
            var y = BitConverter.ToSingle(e, sizeof(float));
            var z = BitConverter.ToSingle(e, sizeof(float));
            var reading = new Vector3(x, y, z);
            OnReadingUpdated?.Invoke(this, reading);
        }

        private void ConnectionHandler_OnReceived(object sender, byte[] e)
        {
            TestJitterFixer.Enqueue(e);
            
        }

        public async Task UpdateReadingAsync(Vector3 reading)
        {
            LatestReading = reading;
            await ConnectionHandler.SendAsync(EncodedLatestReading);
        }
    }
}
