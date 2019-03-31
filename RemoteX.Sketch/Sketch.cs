using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RemoteX.Sketch
{
    public class Sketch
    {
        public SketchEngine SketchEngine { get; }
        public SkiaManager SkiaManager { get; }
        public Task RunSketchTask;
        public Timer UpdateTimer { get; }
        public float DesiredFrameRate { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        
        
        public Sketch()
        {
            SketchEngine = new SketchEngine();
            DesiredFrameRate = 60;
            UpdateTimer = new Timer();
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Interval = 1 / DesiredFrameRate * 1000;
            SkiaManager = SketchEngine.Instantiate<SkiaManager>();


        }
        
        public void Start()
        {
            RunSketchTask = _UpdataTask();
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        public Task _UpdataTask()
        {
            Task task = Task.Run(() =>{
                TimeSpan frameTimer = TimeSpan.Zero;
                DateTime lastFrameDateTime = DateTime.Now;
                TimeSpan frameRateTimeSpan = new TimeSpan(0, 0, 0, 0, (int)(1 / DesiredFrameRate * 1000));
                while (true)
                {
                    DateTime currDateTime = DateTime.Now;
                    frameTimer += (currDateTime - lastFrameDateTime);
                    lastFrameDateTime = currDateTime;
                    if (frameTimer >= frameRateTimeSpan)
                    {
                        SketchEngine.Update((float)frameTimer.TotalSeconds);
                        frameTimer = TimeSpan.Zero;
                    }
                }
            });
            return task;
        }


    }
}
