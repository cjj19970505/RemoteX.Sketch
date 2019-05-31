using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RemoteX.Sketch.Editor.ComponentBuilder
{
    public class InputComponentBuilder : SketchObject, ISkiaRenderer
    {
        public RectTransform RectTransform { get; private set; }
        public Page InspectorPage { get; set; }
        public int Level { get; set; }

        public virtual IArea SelecteArea { get; }

        public (RectArea Right, RectArea Up, RectArea Left, RectArea Down) RescaleDetectArea
        {
            get
            {
                float rescaleBarWidth = 20;
                (Vector2 Min, Vector2 Max) right = (new Vector2(RectTransform.Rect.Max.X - rescaleBarWidth / 2, RectTransform.Rect.Min.Y), new Vector2(RectTransform.Rect.Max.X + rescaleBarWidth / 2, RectTransform.Rect.Max.Y));
                (Vector2 Min, Vector2 Max) up = (new Vector2(RectTransform.Rect.Min.X, RectTransform.Rect.Max.Y - rescaleBarWidth / 2), new Vector2(RectTransform.Rect.Max.X + rescaleBarWidth / 2, RectTransform.Rect.Max.Y));
                (Vector2 Min, Vector2 Max) left = (new Vector2(RectTransform.Rect.Min.X - rescaleBarWidth / 2, RectTransform.Rect.Min.Y), new Vector2(RectTransform.Rect.Min.X + rescaleBarWidth / 2, RectTransform.Rect.Max.Y));
                (Vector2 Min, Vector2 Max) down = (new Vector2(RectTransform.Rect.Min.X, RectTransform.Rect.Min.Y - rescaleBarWidth / 2), new Vector2(RectTransform.Rect.Max.X + rescaleBarWidth / 2, RectTransform.Rect.Min.Y));
                return (new RectArea(right), new RectArea(up), new RectArea(left), new RectArea(down));
            }
        }

        public InputComponentBuilder() : base()
        {

        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnInstantiated()
        {
            base.OnInstantiated();
            var sketchInfo = SketchEngine.FindObjectByType<SketchInfo>();
            RectTransform = new RectTransform(sketchInfo);
        }
        protected override void Start()
        {
            base.Start();
        }
        SKPaint _ResizeBarFill = new SKPaint
        {
            Color = SKColors.AliceBlue
        };
        void ISkiaRenderer.PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {
            

            SKRect canvasRightBar = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRect(RescaleDetectArea.Right.Rect.ToSKRect());
            SKRect canvasUpBar = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRect(RescaleDetectArea.Up.Rect.ToSKRect());
            SKRect canvasLeftBar = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRect(RescaleDetectArea.Left.Rect.ToSKRect());
            SKRect canvasDownBar = skiaManager.SketchSpaceToCanvasSpaceMatrix.MapRect(RescaleDetectArea.Down.Rect.ToSKRect());
            canvas.DrawRect(canvasRightBar, _ResizeBarFill);
            canvas.DrawRect(canvasUpBar, _ResizeBarFill);
            canvas.DrawRect(canvasLeftBar, _ResizeBarFill);
            canvas.DrawRect(canvasDownBar, _ResizeBarFill);
        }
    }

    public class ExampleInputComponentBuilder : InputComponentBuilder
    {

    }
}
