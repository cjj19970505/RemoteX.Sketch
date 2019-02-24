using RemoteX.Sketch.CoreModule;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.Skia
{
    public class GridRenderer : SketchObject, ISkiaRenderer
    {
        public float IntervalX { get; set; }
        public float IntervalY { get; set; }

        readonly SKPaint _TextPaint = new SKPaint
        {
            TextSize = 20,
            Color = SKColors.White,
            TextAlign = SKTextAlign.Left
        };
        readonly SKPaint _LinePaint = new SKPaint
        {
            Color = SKColors.Gray,
            StrokeWidth = 1
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            IntervalX = 200;
            IntervalY = 200;
            SKPoint horizonStep = new SKPoint(IntervalX, 0);
            SKPoint verticleStep = new SKPoint(0, IntervalY);

            var localClipBounds = canvas.LocalClipBounds;

            skiaManager.SketchSpaceToCanvasSpaceMatrix.TryInvert(out SKMatrix skiaToSketch);

            for (int i = 0; IntervalX * i < localClipBounds.Width; i++)
            {
                var point0 = new SKPoint(i * IntervalX, 0);
                var point1 = new SKPoint(i * IntervalX, canvas.LocalClipBounds.Height);
                canvas.DrawLine(point0, point1, _LinePaint);
            }
            for (int j = 0; IntervalY * j < localClipBounds.Height; j++)
            {
                var point0 = new SKPoint(0, IntervalY * j);
                var point1 = new SKPoint(canvas.LocalClipBounds.Width, IntervalY * j);
                canvas.DrawLine(point0, point1, _LinePaint);
            }
            for (int i = 0; IntervalX * i < localClipBounds.Width; i++)
            {
                for (int j = 0; IntervalY * j < localClipBounds.Height; j++)
                {
                    var point = new SKPoint(i * IntervalX, j * IntervalY);
                    
                    canvas.DrawText(skiaToSketch.MapPoint(point).ToString(), point, _TextPaint);
                }
            }
            
            

            


        }
    }
}
