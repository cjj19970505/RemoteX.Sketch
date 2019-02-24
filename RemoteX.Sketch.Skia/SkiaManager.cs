using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        public Action InvalidateView { get; set; }
        public SKMatrix SketchSpaceToCanvasSpaceMatrix { get; set; }

        /// <summary>
        /// Used to adjust the AppToSketch argument such as SpaceMatrixs
        /// </summary>
        public event EventHandler<SKCanvas> BeforePaint;


        public void Init(Action invalidateViewFunc, SKMatrix sketchSpaceToCanvasSpaceMatrix)
        {
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
            InvalidateView();
        }

        [Obsolete("这边的操作可能不线程安全")]
        public void OnPaintSurface(SKCanvas canvas)
        {
            if (!IsInstantiated)
            {
                return;
            }
            SKPaint paint = new SKPaint()
            {
                TextSize = 10,
                TextAlign = SKTextAlign.Left,
                Color = SKColors.White
            };
            paint.TextSize = 20;
            canvas.Clear();

            BeforePaint?.Invoke(this, canvas);

            //这个操作可能不安全
            foreach (var skiaObject in SketchEngine.SketchObjectList)
            {
                if (skiaObject is ISkiaRenderer)
                {
                    (skiaObject as ISkiaRenderer).PaintSurface(this, canvas);
                }
            }
        }
    }
}
