
using RemoteX.Input.Win10;
using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace RemoteX.Sketch.UwpExample
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Sketch Sketch { get; }
        SketchInputManager sketchInputManager;
        public MainPage()
        {
            this.InitializeComponent();
            InputManager inputManager = new InputManager(InputLayerRect);

            Sketch = new Sketch();
            Sketch.SkiaManager.Init(SKCanvasView.Invalidate, SKMatrix.MakeScale(1, -1));
            System.Diagnostics.Debug.WriteLine("MainPageThread:" + Thread.CurrentThread.ManagedThreadId);
            Sketch.Start();
            Sketch.SketchEngine.Instantiate<ExampleSketchObject>();
            Sketch.SketchEngine.Instantiate<GridRenderer>();
            Sketch.SketchEngine.Instantiate<PointerInfoBoard>();
            sketchInputManager = Sketch.SketchEngine.Instantiate<SketchInputManager>();
            sketchInputManager.Init(inputManager);
            //InputLayerRect.TransformMatrix;
            Matrix3x2 matrix = Matrix3x2.CreateScale(0.2f, -0.2f);

            sketchInputManager.InputSpaceToSketchSpaceMatrix = matrix;
            

            Sketch.SkiaManager.BeforePaint += SkiaManager_BeforePaint;
            
        }

        private void SkiaManager_BeforePaint(object sender, SKCanvas e)
        {
            var skiaManager = sender as SkiaManager;
            SKMatrix.MakeTranslation(0, e.LocalClipBounds.Height);
            var matrix = skiaManager.SketchSpaceToCanvasSpaceMatrix;
            matrix.SetScaleTranslate(0.2f, -0.2f, e.LocalClipBounds.Width / 2, e.LocalClipBounds.Height/2);
            skiaManager.SketchSpaceToCanvasSpaceMatrix = matrix;

            Matrix3x2 mat = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-e.LocalClipBounds.Width / 2, -e.LocalClipBounds.Height / 2), Matrix3x2.CreateScale(1/0.2f, -1/0.2f));

            sketchInputManager.InputSpaceToSketchSpaceMatrix = mat;

        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs e)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 24
            };
            Sketch.SkiaManager.OnPaintSurface(e.Surface.Canvas);
            
        }
    }
}
