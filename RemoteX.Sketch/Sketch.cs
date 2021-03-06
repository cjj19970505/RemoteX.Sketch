﻿using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RemoteX.Sketch
{
    public class Sketch
    {
        public SketchEngine SketchEngine { get; }
        public SkiaManager SkiaManager { get; }
        public SketchInfo SketchInfo { get; }
        public Task RunSketchTask;
        public float DesiredFrameRate { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        /// <summary>
        /// 当在进行Update时这个锁会锁住，若害怕自己的异步操作会有不良影响的话注意锁住
        /// </summary>
        public object UpdateTheadLock { get; }
        public Matrix3x2 SketchToSketchNormalizedMatrix
        {
            get
            {
                return Matrix3x2.CreateScale(1 / Width, 1 / Height);
            }
        }


        public Sketch()
        {
            SketchEngine = new SketchEngine();
            UpdateTheadLock = new object();
            DesiredFrameRate = 60;
            SkiaManager = SketchEngine.Instantiate<SkiaManager>();
            SketchInfo = SketchEngine.Instantiate<SketchInfo>();
            SketchInfo.Init(this);
            Width = 1000;
            Height = 200;
        }
        
        public void Start()
        {
            SketchEngine.Update(0);
            //RunSketchTask = _UpdataTask();
            RunSketchTask = _RunTask();
        }

        public Task _RunTask()
        {
            Task task = Task.Run(() =>
            {
                SketchEngine.Start();
            });
            return task;
        }

        public Task _UpdataTask()
        {
            Task task = Task.Run(() =>{
                TimeSpan frameTimer = TimeSpan.Zero;
                DateTime lastFrameDateTime = DateTime.Now;
                TimeSpan frameRateTimeSpan = new TimeSpan(0, 0, 0, 0, (int)(1 / DesiredFrameRate * 1000));
                while (true)
                {
                    DateTime startUpdateDateTime = DateTime.Now;
                    lock(UpdateTheadLock)
                    {
                        SketchEngine.Update((float)frameTimer.TotalSeconds);
                    }
                    var updateTimeSpan = DateTime.Now - startUpdateDateTime;
                    var sleepTimespan = frameRateTimeSpan - updateTimeSpan;
                    if(sleepTimespan > TimeSpan.FromMilliseconds(0))
                    {
                        System.Threading.Thread.Sleep((int)sleepTimespan.TotalMilliseconds);
                    }
                    
                }
            });
            return task;
        }

    }
    public class SketchInfo : SketchObject, ISkiaRenderer
    {
        public Sketch Sketch { get; private set; }
        public event EventHandler OnUpdated;
        public event EventHandler<OnDrawEventArgs> OnDraw;
        public void Init(Sketch sketch)
        {
            Sketch = sketch;
        }

        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            OnDraw?.Invoke(this, new OnDrawEventArgs(skiaManager, canvas));
        }

        protected override void Update()
        {
            base.Update();
            OnUpdated?.Invoke(this, null);
        }

        public class OnDrawEventArgs:EventArgs
        {
            public SkiaManager SkiaManager { get; }
            public SKCanvas SKCanvas { get; }
            public OnDrawEventArgs(SkiaManager skiaManager, SKCanvas canvas)
            {
                SkiaManager = skiaManager;
                SKCanvas = canvas;
            }
        }
    }
}
