using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteX.Sketch.UwpExample
{
    public class TestJitterFixer
    {
        private object BufferQueueLock;
        public Queue<byte[]> BufferQueue;
        public event EventHandler<byte[]> DataEmited;
        private Thread EmitTaskThread;

        public TimeSpan InitEmitTimeSpan { get; }

        public int StablizedBufferCount { get; }
        private TimeSpan guessInterval;
        private List<TimeSpan> OriginTimeSpanList { get; }
        public TestJitterFixer()
        {
            BufferQueue = new Queue<byte[]>();
            OriginTimeSpanList = new List<TimeSpan>();
            BufferQueueLock = new object();
            InitEmitTimeSpan = TimeSpan.FromMilliseconds(0);
            StablizedBufferCount = 1;
            RunEmitTask();
        }
        DateTime _PreviousEnqueueDateTime = DateTime.Now;
        public void Enqueue(byte[] data)
        {
            var now = DateTime.Now;
            if(now - _PreviousEnqueueDateTime < TimeSpan.FromMilliseconds(500))
            {
                OriginTimeSpanList.Add(now - _PreviousEnqueueDateTime);
            }
            _PreviousEnqueueDateTime = now;
            if(OriginTimeSpanList.Count > 100)
            {
                int aveCount = 20;
                var timeSpanRange = OriginTimeSpanList.GetRange(1, aveCount).ToArray();
                OriginTimeSpanList.RemoveRange(1, aveCount);
                double msSum = 0;
                foreach(var timeSpan in timeSpanRange)
                {
                    msSum += timeSpan.TotalMilliseconds;
                }
                guessInterval = TimeSpan.FromMilliseconds(msSum / aveCount);
            }
            guessInterval = TimeSpan.FromMilliseconds(1000.0 / 40);

            lock (BufferQueueLock)
            {
                BufferQueue.Enqueue(data);
            }
        }
        private Task RunEmitTask()
        {
            return Task.Run(() =>
            {
                EmitTaskThread = Thread.CurrentThread;
                TimeSpan emitInterval = InitEmitTimeSpan;
                DateTime previousEmitDateTime = DateTime.Now;
                float p = 1;
                float minError = -5;
                float maxError = 1;
                while (true)
                {
                    int bufferCount;
                    lock (BufferQueueLock)
                    {
                        bufferCount = BufferQueue.Count;
                    }
                    if(bufferCount == 0)
                    {
                        Thread.Sleep(2);
                        continue;
                    }
                    float error = StablizedBufferCount - bufferCount;
                    if(error > minError && error<maxError)
                    {
                        emitInterval = guessInterval;
                    }
                    else
                    {
                        emitInterval = TimeSpan.FromMilliseconds(p * Math.Exp(error));
                    }
                    
                    if(emitInterval.TotalMilliseconds > 0)
                    {
                        DateTime startSleepDateTime = DateTime.Now;
                        TimeSpan sleepTimeSpan = emitInterval;
                        Thread.Sleep(sleepTimeSpan);
                    }
                    byte[] readyToEmitData;
                    lock (BufferQueueLock)
                    {
                        readyToEmitData = BufferQueue.Dequeue();
                    }
                    DataEmited?.Invoke(this, readyToEmitData);
                }
            });
        }

        


    }
}
