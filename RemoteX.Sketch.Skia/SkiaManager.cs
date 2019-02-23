using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Sketch.CoreModule;
using SkiaSharp;

namespace RemoteX.Sketch.Skia
{
    public class SkiaManager : SketchObject
    {
        /// <summary>
        /// This is for APP To set
        /// Do not set this in sketch
        /// </summary>
        public SKCanvas Canvas { get; set; }
        public Action InvalidateView { get; set; }
        public SKMatrix44 SketchSpaceToCanvasSpaceMatrix { get; set; }

        public void Init(SKCanvas canvas, Action invalidateViewFunc, SKMatrix44 sketchSpaceToCanvasSpaceMatrix)
        {
            Canvas = canvas;
            InvalidateView = invalidateViewFunc;
            SketchSpaceToCanvasSpaceMatrix = sketchSpaceToCanvasSpaceMatrix;
        }

        protected override void OnInstantiated()
        {
            base.OnInstantiated();
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
