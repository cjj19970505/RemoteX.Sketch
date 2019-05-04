using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteX.Sketch.XamExapmple
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //AbsoluteLayout.SetLayoutFlags(CanvasView, AbsoluteLayoutFlags.All);
            //AbsoluteLayout.SetLayoutBounds(CanvasView, new Rectangle(0, 0, 1, 1));

            Sketch = new Sketch();
            Sketch.SkiaManager.Init(CanvasView.InvalidateSurface, SKMatrix.MakeScale(1, -1));
            Sketch.Start();
            Sketch.SketchEngine.Instantiate<ExampleSketchObject>();
            Sketch.SketchEngine.Instantiate<GridRenderer>();
            Sketch.SkiaManager.BeforePaint += SkiaManager_BeforePaint;

            Device.StartTimer(TimeSpan.FromSeconds(1 / 60f), () => { CanvasView.InvalidateSurface(); return !true; });
        }

        private void SkiaManager_BeforePaint(object sender, SKCanvas e)
        {
            var skiaManager = sender as SkiaManager;
            SKMatrix.MakeTranslation(0, e.LocalClipBounds.Height);
            var matrix = skiaManager.SketchSpaceToCanvasSpaceMatrix;
            matrix.SetScaleTranslate(0.2f, -0.2f, e.LocalClipBounds.Width / 2, e.LocalClipBounds.Height / 2);
            skiaManager.SketchSpaceToCanvasSpaceMatrix = matrix;
        }

        public Sketch Sketch { get; private set; }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Blue);

            Sketch.SkiaManager.OnPaintSurface(e.Surface.Canvas);
        }
    }
}
