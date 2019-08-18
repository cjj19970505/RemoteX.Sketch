using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch
{
    public class PointerInfoBoard : SketchObject, ISkiaRenderer
    {
        SKPaint _BoardPaint = new SKPaint()
        {
            Color = SKColors.Red
        };

        SKPaint _BoardFontPaint = new SKPaint()
        {
            TextSize = 20,
            Color = SKColors.Green,
            TextAlign = SKTextAlign.Left
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            var sketchInputManager = SketchEngine.FindObjectByType<SketchInputManager>();
            canvas.DrawRect(0, 0, 200, 200, _BoardPaint);
            foreach(var pointer in sketchInputManager.SketchPointers)
            {
                //Vector2 pos = Vector2.Transform(pointer.Point, skiaManager.SketchSpaceToCanvasSpaceMatrix);
                SKPoint pos = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(new SKPoint(pointer.Point.X, pointer.Point.Y));
                canvas.DrawText(pointer.ToString(), pos, _BoardFontPaint);
            }
        }
    }
}
