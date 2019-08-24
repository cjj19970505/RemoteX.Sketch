using RemoteX.Bluetooth.LE.Gatt.Server;
using RemoteX.Bluetooth.Rfcomm;
using RemoteX.Sketch.InputComponent;
using RemoteX.Sketch.Skia;
using RemoteX.Sketch.XamExapmple.BleServices;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using RemoteX.Sketch.Bluetooth;
using RemoteX.Bluetooth.Procedure.Server;

namespace RemoteX.Sketch.XamExapmple
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        IManagerManager ManagerManager { get; }
        SketchInputManager SketchInputManager { get; }

        ExampleSketchObject ExampleSketchObject;
        GyroscopeRfcommServiceConnectionWrapper GyroscopeRfcommServiceConnectionWrapper;
        KeyboardServiceWrapper KeyboardServiceWrapper;
        public MainPage()
        {

            InitializeComponent();
            //AbsoluteLayout.SetLayoutFlags(CanvasView, AbsoluteLayoutFlags.All);
            //AbsoluteLayout.SetLayoutBounds(CanvasView, new Rectangle(0, 0, 1, 1));
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            var deviceInfomationService = new DeviceInfomationServiceBuilder(bluetoothManager).Build();
            
            bluetoothManager.GattSever.AddService(new DeviceInfomationServiceBuilder(bluetoothManager).Build());
            //bluetoothManager.GattSever.AddService(new BatteryServiceWrapper(bluetoothManager).GattServerService);
            bluetoothManager.GattSever.AddService(new RfcommServerServiceWrapper(bluetoothManager).GattServerService);
            KeyboardServiceWrapper = new KeyboardServiceWrapper(bluetoothManager);
            bluetoothManager.GattSever.AddService(KeyboardServiceWrapper.GattServerService);
            bluetoothManager.GattSever.StartAdvertising();

            var createServiceProviderTask = bluetoothManager.CreateRfcommServiceProviderAsync(GyroscopeRfcommServiceConnectionWrapper.RfcommServiceId);
            createServiceProviderTask.Wait();
            var gyroRfcommServiceProvider = createServiceProviderTask.Result;
            gyroRfcommServiceProvider.OnConnectionReceived += GyroRfcommServiceProvider_OnConnectionReceived;
            gyroRfcommServiceProvider.StartAdvertising();
            Sketch = new Sketch();
            Sketch.SkiaManager.Init(CanvasView.InvalidateSurface, SKMatrix.MakeScale(1, -1));
            
            
            Sketch.SketchEngine.Instantiate<GridRenderer>();
            
            Sketch.SketchEngine.Instantiate<PointerInfoBoard>();
            Sketch.SkiaManager.BeforePaint += SkiaManager_BeforePaint;
            ManagerManager = DependencyService.Get<IManagerManager>();
            SketchInputManager = Sketch.SketchEngine.Instantiate<SketchInputManager>();
            Sketch.Start();
            SketchInputManager.Init(ManagerManager.InputManager);
            
            /*
            var joystick = Sketch.SketchEngine.Instantiate<ColorJoystick>();
            joystick.RectTransform.AnchorMax = new Vector2(1, 1);
            joystick.RectTransform.AnchorMin = new Vector2(0, 0);
            joystick.RectTransform.OffsetMax = new Vector2(-800, -800);
            joystick.RectTransform.OffsetMin = new Vector2(10, 10);
            joystick.Level = 2;
            */

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

            var keyboardButton = Sketch.SketchEngine.Instantiate<KeyboardKeyButton>();
            keyboardButton.RectTransform.AnchorMax = new Vector2(0, 0);
            keyboardButton.RectTransform.AnchorMin = new Vector2(0, 0);
            keyboardButton.RectTransform.OffsetMin = new Vector2(50, 50);
            keyboardButton.RectTransform.OffsetMax = new Vector2(150, 150);
            keyboardButton.Level = 2;
            ExampleSketchObject = Sketch.SketchEngine.Instantiate<ExampleSketchObject>();
            Sketch.SketchEngine.Instantiate<SketchBorderRenderer>();
            Sketch.SketchEngine.Instantiate<RectTransformFrameRenderer>();

            CanvasView.InvalidateSurface();
            ExampleSketchObject.Position = new SKPoint(Sketch.Width/2, Sketch.Height/2);
            SensorSpeed speed = SensorSpeed.Game;
            
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            Gyroscope.Start(speed);

            Device.StartTimer(TimeSpan.FromSeconds(1 / 60f), () => { CanvasView.InvalidateSurface(); return !true; });
        }

        private void Joystick2_OnAreaStatusChanged(object sender, AreaJoystick<byte>.AreaStatusChangeEventArgs<byte> e)
        {
            //KeyboardServiceWrapper.UpdateKeyStatus(e.Area.AreaIdentifier, (KeyStatus)e.NewStatus);
        }

        private void GyroRfcommServiceProvider_OnConnectionReceived(object sender, IRfcommConnection e)
        {
            GyroscopeRfcommServiceConnectionWrapper = new GyroscopeRfcommServiceConnectionWrapper(e);
            //_ = GyroscopeRfcommServiceConnectionWrapper.UpdateReadingAsync(new Vector3(100, 100, 100));
        }

        private DateTime _PreviousReadDateTime = DateTime.Now;
        private async void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            if(GyroscopeRfcommServiceConnectionWrapper == null)
            {
                return;
            }
            var timeSpan = (DateTime.Now - _PreviousReadDateTime).TotalMilliseconds;
            /*
            if(timeSpan < 1000.0/40)
            {
                return;
            }
            */
            _PreviousReadDateTime = DateTime.Now;
            var delta = new SKPoint(e.Reading.AngularVelocity.X*10, e.Reading.AngularVelocity.Y*10);
            ExampleSketchObject.Position = ExampleSketchObject.Position + delta;
            await GyroscopeRfcommServiceConnectionWrapper.UpdateReadingAsync(e.Reading.AngularVelocity);
        }

        private void SkiaManager_BeforePaint(object sender, SKCanvas e)
        {
            var skiaManager = sender as SkiaManager;
            SKMatrix.MakeTranslation(0, e.LocalClipBounds.Height);
            var matrix = skiaManager.SketchSpaceToCanvasSpaceMatrix;
            Sketch.Width = 1600;
            Sketch.Height = 900;
            
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
            Matrix3x2 pxToSketchSpace = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-xTranslate, -yTranslate), Matrix3x2.CreateScale(1/xFactor, -1/yFactor));
            SketchInputManager.InputSpaceToSketchSpaceMatrix = Matrix3x2.Multiply(epxToPx, pxToSketchSpace);
        }

        public Sketch Sketch { get; private set; }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Blue);

            Sketch.SkiaManager.OnPaintSurface(e.Surface.Canvas);
        }
    }
}
