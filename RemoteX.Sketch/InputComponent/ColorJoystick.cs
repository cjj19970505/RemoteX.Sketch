using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    public class ColorJoystick : Joystick, ISkiaRenderer
    {
        protected override IArea StartRegion
        {
            get
            {
                (Vector2 Min, Vector2 Max) = RectTransform.Rect;
                float width = Math.Abs(Max.X - Min.X);
                float height = Math.Abs(Max.Y - Min.Y);
                return new CircleArea
                {
                    Radius = Math.Min(width, height)/2,
                    Position = (Max + Min) / 2
                };
            }
        }
        SKPaint paint = new SKPaint()
        {
            Color = SKColors.AliceBlue,
            Style = SKPaintStyle.Fill
        };
        SKPaint dragPaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Khaki
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            (Vector2 Min, Vector2 Max) = RectTransform.Rect;
            //SKRect rect = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRect(new SKRect(Min.X, Min.Y, Max.X, Max.Y));
            //canvas.DrawRect(rect, dragPaint);
            float radius = ((CircleArea)StartRegion).Radius;
            SKPoint pos = ((CircleArea)StartRegion).Position.ToSKPoint();
            canvas.DrawCircle(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(pos), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRadius(radius), paint);
            if (Pressed)
            {
                float Direction = (float)(Math.Atan2(Delta.Y, Delta.X) * 180 / Math.PI);
                float factor = 0;
                float Distance = Delta.Length();
                if (Direction < 0)
                {
                    factor = (Direction + 360) / 360;
                }
                else
                {
                    factor = (Direction) / 360;
                }
                SKColor baseColor = new SKColor(255, (byte)(255 * factor), 0);
                dragPaint.Color = baseColor;
                canvas.DrawCircle(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(((CircleArea)StartRegion).Position.ToSKPoint()), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRadius(Distance), dragPaint);
            }

        }

        protected override void OnJoystickPressed()
        {
            //throw new NotImplementedException();
        }

        protected override void OnJoystickUp()
        {
            //throw new NotImplementedException();
        }
    }
}
