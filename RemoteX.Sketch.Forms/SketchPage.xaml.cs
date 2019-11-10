using RemoteX.Input;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteX.Sketch.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchPage : ContentPage
    {
        public Sketch Sketch { get; }
        public IInputManager InputManager { get; }
        public SketchInputManager SketchInputManager { get; }
        protected SkiaManager SkiaManager { get; private set; }
        protected SKCanvas SKCanvas { get; private set; }

        public Vector2 SketchSize { get; protected set; }
        public SketchPage(IInputManager inputManager)
        {
            InitializeComponent();
            Sketch = new Sketch();
            Sketch.SkiaManager.Init(CanvasView.InvalidateSurface, SKMatrix.MakeScale(1, -1));
            Sketch.SketchInfo.OnUpdated += SketchInfo_OnUpdated;
            Sketch.SketchInfo.OnDraw += SketchInfo_OnDraw;
            Sketch.SketchEngine.Instantiate<GridRenderer>();
            Sketch.SketchEngine.Instantiate<PointerInfoBoard>();
            Sketch.SkiaManager.BeforePaint += SkiaManager_BeforePaint;
            SketchInputManager = Sketch.SketchEngine.Instantiate<SketchInputManager>();
            Sketch.Start();

            SketchInputManager.Init(inputManager);
            CanvasView.InvalidateSurface();
            Setup();
        }

        private void SketchInfo_OnDraw(object sender, SketchInfo.OnDrawEventArgs e)
        {
            SkiaManager = e.SkiaManager;
            SKCanvas = e.SKCanvas;
            Draw();
        }

        private void SketchInfo_OnUpdated(object sender, EventArgs e)
        {
            Update();
        }

        private void SkiaManager_BeforePaint(object sender, SkiaSharp.SKCanvas e)
        {
            var skiaManager = sender as SkiaManager;
            SKMatrix.MakeTranslation(0, e.LocalClipBounds.Height);
            var matrix = skiaManager.SketchSpaceToCanvasSpaceMatrix;
            Sketch.Width = SketchSize.X;
            Sketch.Height = SketchSize.Y;

            SKPoint sketchSize = new SKPoint(Sketch.Width, Sketch.Height);

            //matrix.SetScaleTranslate(1f, -1f, e.LocalClipBounds.Width / 2, e.LocalClipBounds.Height / 2);

            var sketchRatio = sketchSize.X / sketchSize.Y;
            var localClipRatio = e.LocalClipBounds.Width / e.LocalClipBounds.Height;
            var xFactor = e.LocalClipBounds.Width / sketchSize.X;
            var yFactor = e.LocalClipBounds.Height / sketchSize.Y;
            if (localClipRatio > sketchRatio)
            {
                xFactor = yFactor;
            }
            else
            {
                yFactor = xFactor;
            }
            var xTranslate = e.LocalClipBounds.MidX - (xFactor * sketchSize.X) / 2;
            var yTranslate = e.LocalClipBounds.Height - (e.LocalClipBounds.MidY - (yFactor * sketchSize.Y) / 2);
            matrix.SetScaleTranslate(xFactor, -yFactor, xTranslate, yTranslate);
            skiaManager.SketchSpaceToCanvasSpaceMatrix = matrix;

            Matrix3x2 epxToPx = Matrix3x2.CreateScale(1);
            Matrix3x2 pxToSketchSpace = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-xTranslate, -yTranslate), Matrix3x2.CreateScale(1 / xFactor, -1 / yFactor));
            SketchInputManager.InputSpaceToSketchSpaceMatrix = Matrix3x2.Multiply(epxToPx, pxToSketchSpace);
        }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Blue);
            Sketch.SkiaManager.OnPaintSurface(e.Surface.Canvas);
        }

        protected virtual void Setup()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void Draw()
        {

        }
    }
}