using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch
{
    public class SketchBorderRenderer : SketchObject, ISkiaRenderer
    {
        readonly SKPaint _BorderPaint = new SKPaint
        {
            Color = SKColors.Green,
            StrokeWidth = 10
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            var sketchInfo = SketchEngine.FindObjectByType<SketchInfo>();
            SKPoint leftDown = new SKPoint(0, 0);
            SKPoint leftUp = new SKPoint(0, sketchInfo.Sketch.Height);
            SKPoint rightUp = new SKPoint(sketchInfo.Sketch.Width, sketchInfo.Sketch.Height);
            SKPoint rightDown = new SKPoint(sketchInfo.Sketch.Width, 0);
            canvas.DrawLine(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(leftDown), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(leftUp), _BorderPaint);
            canvas.DrawLine(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(leftUp), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(rightUp), _BorderPaint);
            canvas.DrawLine(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(rightUp), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(rightDown), _BorderPaint);
            canvas.DrawLine(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(rightDown), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(leftDown), _BorderPaint);

        }
    }
}
