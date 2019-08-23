using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    public class KeyboardKeyButton:Button, ISkiaRenderer
    {
        public override IArea StartRegion
        {
            get
            {
                return new RectArea(RectTransform.Rect);
            }
        }

        public event EventHandler KeyDown;
        public event EventHandler KeyUp;
        public override void OnPressed()
        {
            base.OnPressed();
            KeyDown?.Invoke(this, null);
        }
        public override void OnReleased()
        {
            base.OnReleased();
            KeyUp?.Invoke(this, null);
        }

        SKPaint ReleasedPaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Blue
        };
        SKPaint PressedPaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Red
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            
        }
    }
}
