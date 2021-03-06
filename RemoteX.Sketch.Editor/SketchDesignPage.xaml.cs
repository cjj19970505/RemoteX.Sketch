﻿using RemoteX.Input.Win10;
using RemoteX.Sketch.Editor.ComponentBuilder;
using RemoteX.Sketch.InputComponent;
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
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace RemoteX.Sketch.Editor
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SketchDesignPage : Page
    {
        public Sketch Sketch { get; }
        SketchInputManager sketchInputManager;
        public SketchDesignPage()
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
            Sketch.SketchEngine.Instantiate<SketchBorderRenderer>();
            Sketch.SketchEngine.Instantiate<ComponentSelector>();


            sketchInputManager = Sketch.SketchEngine.Instantiate<SketchInputManager>();
            sketchInputManager.Init(inputManager);
            var joystick = Sketch.SketchEngine.Instantiate<ColorJoystick>();
            joystick.RectTransform.AnchorMax = new Vector2(1, 1);
            joystick.RectTransform.AnchorMin = new Vector2(0, 0);
            joystick.RectTransform.OffsetMax = new Vector2(-100, -100);
            joystick.RectTransform.OffsetMin = new Vector2(100, 100);
            joystick.Level = 1;

            var inputComponentBuilder = Sketch.SketchEngine.Instantiate<InputComponentBuilder>();
            inputComponentBuilder.RectTransform.AnchorMax = new Vector2(1, 1);
            inputComponentBuilder.RectTransform.AnchorMin = new Vector2(0, 0);
            inputComponentBuilder.RectTransform.OffsetMax = new Vector2(-100, -100);
            inputComponentBuilder.RectTransform.OffsetMin = new Vector2(100, 100);

            var joystick2 = Sketch.SketchEngine.Instantiate<ColorJoystick>();
            joystick2.RectTransform.AnchorMax = new Vector2(0, 0);
            joystick2.RectTransform.AnchorMin = new Vector2(0, 0);
            joystick2.RectTransform.OffsetMax = new Vector2(-10, -10);
            joystick2.RectTransform.OffsetMin = new Vector2(-2000, 2000);
            joystick2.Level = 2;
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
            matrix.SetScaleTranslate(0.2f, -0.2f, e.LocalClipBounds.Width / 2, e.LocalClipBounds.Height / 2);
            skiaManager.SketchSpaceToCanvasSpaceMatrix = matrix;


            float baseDpi = 96;
            Matrix3x2 epxToPx = Matrix3x2.CreateScale(DisplayInformation.GetForCurrentView().LogicalDpi / baseDpi);
            Matrix3x2 pxToSketchSpace = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-e.LocalClipBounds.Width / 2, -e.LocalClipBounds.Height / 2), Matrix3x2.CreateScale(1 / 0.2f, -1 / 0.2f));
            sketchInputManager.InputSpaceToSketchSpaceMatrix = Matrix3x2.Multiply(epxToPx, pxToSketchSpace);
            //System.Diagnostics.Debug.WriteLine(DisplayInformation.GetForCurrentView().);

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
