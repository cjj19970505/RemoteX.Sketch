using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    public class ControllerButton : Button, ISkiaRenderer
    {
        public event EventHandler OnButtonDown;
        public event EventHandler OnButtonUp;
        public override IArea StartRegion
        {
            get
            {
                (Vector2 Min, Vector2 Max) = RectTransform.Rect;
                float width = Math.Abs(Max.X - Min.X);
                float height = Math.Abs(Max.Y - Min.Y);
                return new CircleArea
                {
                    Radius = Math.Min(width, height) / 2,
                    Position = (Max + Min) / 2
                };
            }
        }
        public string ButtonString { get; set; }

        public SKPaint ReleasedShapePaint = new SKPaint
        {
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.White,

        };
        public SKPaint PressedShapePaint = new SKPaint
        {
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.Gray,
        };
        public SKPaint ReleasedStringPaint = new SKPaint
        {
            TextSize = 10,
            IsVerticalText = true,
            TextAlign = SKTextAlign.Center,
            Color = SKColors.Green
        };

        public SKPaint PressedStringPaint = new SKPaint
        {
            TextSize = 10,
            IsVerticalText = true,
            TextAlign = SKTextAlign.Center,
            Color = SKColors.Green
        };

        public override void OnPressed()
        {
            base.OnPressed();
            OnButtonDown?.Invoke(this, null);
        }

        public override void OnReleased()
        {
            base.OnReleased();
            OnButtonUp?.Invoke(this, null);
        }

        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            float radius = ((CircleArea)StartRegion).Radius;
            SKPoint pos = ((CircleArea)StartRegion).Position.ToSKPoint();
            var shapePaint = Pressed ? PressedShapePaint : ReleasedShapePaint;
            var wordPaint = Pressed ? PressedStringPaint : ReleasedStringPaint;
            wordPaint.TextSize = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRadius(((CircleArea)StartRegion).Radius);
            canvas.DrawCircle(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(pos), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRadius(radius), shapePaint);
            canvas.DrawText(ButtonString, skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(((CircleArea)StartRegion).Position.ToSKPoint()), wordPaint);
        }
    }
}
