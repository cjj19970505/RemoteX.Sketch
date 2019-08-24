using RemoteX.Input;
using RemoteX.Sketch.Bluetooth;
using RemoteX.Sketch.Forms;
using RemoteX.Sketch.InputComponent;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.XamExapmple
{
    internal class SketchTest : SketchPage
    {
        public SketchTest(IInputManager inputManager):base(inputManager)
        {

        }

        public ExampleSketchObject ExampleSketchObject { get; private set; }

        protected override void Setup()
        {
            var joystick2 = Sketch.SketchEngine.Instantiate<LineAreaJoystick<byte>>();
            joystick2.RectTransform.AnchorMax = new Vector2(0, 0);
            joystick2.RectTransform.AnchorMin = new Vector2(0, 0);
            joystick2.RectTransform.OffsetMax = new Vector2(600, 600);
            joystick2.RectTransform.OffsetMin = new Vector2(500, 500);
            joystick2.Level = 3;

            joystick2.AddArea(AreaJoystick<byte>.Area<byte>.CreateFromAngle((byte)VirtualKeyCode.VK_D, -60, 60, 0, float.PositiveInfinity));
            joystick2.AddArea(AreaJoystick<byte>.Area<byte>.CreateFromAngle((byte)VirtualKeyCode.VK_W, 30, 150, 0, float.PositiveInfinity));
            joystick2.AddArea(AreaJoystick<byte>.Area<byte>.CreateFromAngle((byte)VirtualKeyCode.VK_A, 120, 240, 0, float.PositiveInfinity));
            joystick2.AddArea(AreaJoystick<byte>.Area<byte>.CreateFromAngle((byte)VirtualKeyCode.VK_S, 210, 330, 0, float.PositiveInfinity));
            joystick2.AddArea(AreaJoystick<byte>.Area<byte>.CreateFromAngle((byte)VirtualKeyCode.LSHIFT, 0, 360, 0.7f, float.PositiveInfinity));
            joystick2.OnAreaStatusChanged += Joystick2_OnAreaStatusChanged;

            var keyboardButton = Sketch.SketchEngine.Instantiate<ControllerButton>();
            keyboardButton.RectTransform.AnchorMax = new Vector2(0, 0);
            keyboardButton.RectTransform.AnchorMin = new Vector2(0, 0);
            keyboardButton.RectTransform.OffsetMin = new Vector2(50, 50);
            keyboardButton.RectTransform.OffsetMax = new Vector2(150, 150);
            keyboardButton.Level = 2;
            keyboardButton.ButtonString = "X";
            Sketch.SketchEngine.Instantiate<SketchBorderRenderer>();
            Sketch.SketchEngine.Instantiate<RectTransformFrameRenderer>();
            
        }
        protected override void Update()
        {
            
        }
        SKPaint paint = new SKPaint
        {
            Color = SKColors.Brown,
            TextSize = 50,
            FakeBoldText = true
        };
        protected override void Draw()
        {
            SKCanvas.DrawText(DateTime.Now.ToString("HH:mm:ss"), 50, 50, paint);
        }

        private void Joystick2_OnAreaStatusChanged(object sender, AreaJoystick<byte>.AreaStatusChangeEventArgs<byte> e)
        {
            //throw new NotImplementedException();
        }
    }
}
