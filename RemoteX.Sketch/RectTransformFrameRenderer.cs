using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch
{
    public class RectTransformFrameRenderer : SketchObject, ISkiaRenderer
    {
        private SKPaint FramePaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Green,
            StrokeWidth = 5
        };
        private SKPaint MinPointPaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 10
        };
        private SKPaint MaxPointPaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            foreach (var skiaObject in SketchEngine.SketchObjectList)
            {
                if (skiaObject is IRectTransformable)
                {
                    var rect = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRect((skiaObject as IRectTransformable).RectTransform.Rect.ToSKRect());
                    canvas.DrawRect(rect, FramePaint);
                    canvas.DrawPoint(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint((skiaObject as IRectTransformable).RectTransform.Rect.Min.ToSKPoint()), MinPointPaint);
                    canvas.DrawPoint(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint((skiaObject as IRectTransformable).RectTransform.Rect.Max.ToSKPoint()), MaxPointPaint);
                }
            }
        }
    }
}
