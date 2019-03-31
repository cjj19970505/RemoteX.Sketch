using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RemoteX.Sketch
{
    public class ExampleSketchObject:SketchObject, ISkiaRenderer
    {
        public SKPoint Position = new SKPoint(0, 0);
        public SKPoint Velocity = new SKPoint(50, 50);
        public void PaintSurface(SkiaManager skiaManager ,SKCanvas canvas)
        {
            //System.Diagnostics.Debug.WriteLine("PaintSurface");
            SKPaint paint = new SKPaint()
            {
                Color = SKColors.Red
            };
            
            canvas.DrawCircle(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(Position), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRadius(50), paint);
        }

        protected override void Update()
        {
            Position = Position + new SKPoint(Velocity.X * SketchEngine.Time.DeltaTime, Velocity.Y * SketchEngine.Time.DeltaTime);

        }
    }
}
