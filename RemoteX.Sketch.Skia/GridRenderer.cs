using RemoteX.Sketch.CoreModule;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.Skia
{
    public class GridRenderer : SketchObject, ISkiaRenderer
    {
        //public float IntervalX { get; set; }
        //public float IntervalY { get; set; }
        public float Base = 100;
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

            var localClipBounds = canvas.LocalClipBounds;
            
            skiaManager.SketchSpaceToCanvasSpaceMatrix.TryInvert(out SKMatrix skiaToSketch);
            var clipBoundSketchSpace = skiaToSketch.MapRect(localClipBounds);
            int widthLevel = (int)Math.Log(clipBoundSketchSpace.Width, Base);
            int heightLevel = (int)Math.Log(clipBoundSketchSpace.Height, Base);
            
            float IntervalX = (float)Math.Pow(Base, widthLevel);
            float IntervalY = (float)Math.Pow(Base, heightLevel);

            float intervalXSketchSpace = (float)Math.Pow(Base, widthLevel);
            float intervalYSketchSpace = (float)Math.Pow(Base, heightLevel);

            SKRectI nRect = new SKRectI
            {
                Left = (int)Math.Ceiling(clipBoundSketchSpace.Left / Math.Pow(Base, widthLevel)),
                Top = (int)Math.Ceiling(clipBoundSketchSpace.Top / Math.Pow(Base, heightLevel)),
                Right = (int)Math.Floor(clipBoundSketchSpace.Right / Math.Pow(Base, widthLevel)),
                Bottom = (int)Math.Floor(clipBoundSketchSpace.Bottom / Math.Pow(Base, heightLevel))
            };

            
            /*
            for (int i = nRect.Bottom;s i <= nRect.Top; i++)
            {
                var point0 = new SKPoint(i * intervalXSketchSpace, 0);
                var point1 = new SKPoint(i * intervalXSketchSpace, canvas.LocalClipBounds.Height);
                canvas.DrawLine(point0, point1, _LinePaint);
            }
            */
            //System.Diagnostics.Debug.WriteLine(nRect);
            for (int i = nRect.Left; i <= nRect.Right; i++)
            {
                for (int j = nRect.Top; j <= nRect.Bottom; j++)
                {
                    var pointSketchSpace = new SKPoint(i * intervalXSketchSpace, j * intervalYSketchSpace);

                    canvas.DrawText(pointSketchSpace.ToString(), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(pointSketchSpace), _TextPaint);
                }
            }

            for (int i = nRect.Left; i <= nRect.Right; i++)
            {
                var point0 = new SKPoint(i * intervalXSketchSpace, clipBoundSketchSpace.Top);
                var point1 = new SKPoint(i * intervalXSketchSpace, clipBoundSketchSpace.Bottom);
                canvas.DrawLine(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(point0), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(point1), _LinePaint);
            }

            for (int j = nRect.Top; j <= nRect.Bottom; j++)
            {
                var point0 = new SKPoint(clipBoundSketchSpace.Left, j * intervalYSketchSpace);
                var point1 = new SKPoint(clipBoundSketchSpace.Right, j * intervalYSketchSpace);
                canvas.DrawLine(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(point0), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(point1), _LinePaint);
            }
            /*
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
            */




        }
    }
}
