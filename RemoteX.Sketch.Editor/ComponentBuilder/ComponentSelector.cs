using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    class ComponentSelector : SketchObject, IInputComponent, ISkiaRenderer
    {
        protected SketchPointer OnSketchPointer { get; private set; }
        public Vector2 Delta { get; private set; }
        public Vector2 _StartPos;
        public bool Pressed { get; private set; }
        public int Level
        {
            get
            {
                return 0;
            }
        }

        public IArea StartRegion
        {
            get
            {
                return new AllArea();
            }
        }

        protected override void Update()
        {
            SketchInputManager sketchInputManager = SketchEngine.FindObjectByType<SketchInputManager>();
            if (OnSketchPointer == null)
            {
                foreach (var sketchPointer in sketchInputManager.SketchPointers)
                {
                    if (StartRegion.IsOverlapPoint(sketchPointer.Point) && sketchPointer.HitLayer == Level)
                    {
                        if (sketchPointer.State == Input.PointerState.Pressed)
                        {
                            OnSketchPointer = sketchPointer;
                            _StartPos = OnSketchPointer.Point;
                            Pressed = true;

                        }
                    }
                }
            }
            else
            {
                if (OnSketchPointer.State == Input.PointerState.Pressed)
                {
                    Delta = OnSketchPointer.Point - _StartPos;

                }
                else
                {
                    Pressed = false;
                    OnSketchPointer = null;
                }
            }
        }
        SKPaint _SelectorPaint = new SKPaint
        {
            Color = new SKColor(0, 255, 125, 50)
        };
        public void PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            if(Pressed)
            {
                SKPoint p1 = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint(_StartPos.ToSKPoint());
                SKPoint p2 = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapPoint((_StartPos + Delta).ToSKPoint());
                SKPoint pMin = new SKPoint();
                SKPoint pMax = new SKPoint();
                if(p1.X <= p2.X && p1.Y <= p2.Y)
                {
                    pMin = p1;
                    pMax = p2;
                }
                else if(p1.X >= p2.X && p1.Y >= p2.Y)
                {
                    pMin = p2;
                    pMax = p1;
                }
                else
                {
                    SKPoint p3 = new SKPoint(p1.X, p2.Y);
                    SKPoint p4 = new SKPoint(p2.X, p1.Y);
                    if (p3.X <= p4.X && p3.Y <= p4.Y)
                    {
                        pMin = p3;
                        pMax = p4;
                    }
                    else
                    {
                        pMin = p4;
                        pMax = p3;
                    }
                }

                SKRect rect = new SKRect(pMin.X, pMin.Y, pMax.X, pMax.Y);
                canvas.DrawRect(rect, _SelectorPaint);
                
            }
        }
    }
}
