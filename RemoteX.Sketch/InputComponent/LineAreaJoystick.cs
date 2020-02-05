using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    public class LineAreaJoystick<T>:AreaJoystick<T>, ISkiaRenderer where T:IComparable
    {
        SkiaManager SkiaManager;
        public LineAreaJoystick():base()
        {

        }

        protected override void Start()
        {
            if (SkiaManager == null)
            {
                SkiaManager = SketchEngine.FindObjectByType<SkiaManager>();
            }
            SkiaManager.InvalidCanvas();
        }
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

        SKPaint IdlePaint = new SKPaint()
        {
            Color = SKColors.Azure,
            Style = SKPaintStyle.Fill
        };
        SKPaint MaxLengthAreaPaint = new SKPaint()
        {
            Color = SKColors.YellowGreen,
            Style = SKPaintStyle.Stroke
        };
        SKPaint LinePaint = new SKPaint()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeWidth = 10
        };
        SKPaint _BoardFontPaint = new SKPaint()
        {
            TextSize = 20,
            Color = SKColors.Green,
            TextAlign = SKTextAlign.Left
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            (Vector2 Min, Vector2 Max) = RectTransform.Rect;
            float radius = ((CircleArea)StartRegion).Radius;
            SKPoint pos = ((CircleArea)StartRegion).Position.ToSKPoint();
            canvas.DrawCircle(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(pos), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRadius(radius), IdlePaint);
            canvas.DrawCircle(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(pos), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRadius(MaxLength), MaxLengthAreaPaint);
            if (Pressed)
            {
                var endPos = (((CircleArea)StartRegion).Position + Delta).ToSKPoint();
                canvas.DrawLine(skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(pos), skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(endPos), LinePaint);

                
                var pointer = OnSketchPointer;
                SKPoint pointerPoint = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(new SKPoint(pointer.Point.X, pointer.Point.Y));
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("POINTER INFO:");
                
                foreach (var area in AreaList.ToArray())
                {
                    if (area.Status == AreaStatus.Pressed)
                    {
                        sb.Append(area.AreaIdentifier + ", ");
                    }
                }
                
                canvas.DrawText(sb.ToString(), pointerPoint, _BoardFontPaint);
                
            }
            
        }

        protected override void OnJoystickPressed()
        {
            base.OnJoystickPressed();
            
            SkiaManager.InvalidCanvas();
        }
        protected override void OnJoystickUp()
        {
            base.OnJoystickUp();
            if (SkiaManager == null)
            {
                SkiaManager = SketchEngine.FindObjectByType<SkiaManager>();
            }
            SkiaManager.InvalidCanvas();
        }

        protected override void OnDeltaChanged()
        {
            base.OnDeltaChanged();
            if (SkiaManager == null)
            {
                SkiaManager = SketchEngine.FindObjectByType<SkiaManager>();
            }
            SkiaManager.InvalidCanvas();
        }
    }
}
