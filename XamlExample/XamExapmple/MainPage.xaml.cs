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
        GyroscopeServiceWrapper GyroscopeServiceWrapper;
        ClientRfcommServiceWrapper ClientRfcommServiceWrapper;
        IRfcommConnection GyroConnection { get; set; }
        public MainPage()
        {

            InitializeComponent();
            //AbsoluteLayout.SetLayoutFlags(CanvasView, AbsoluteLayoutFlags.All);
            //AbsoluteLayout.SetLayoutBounds(CanvasView, new Rectangle(0, 0, 1, 1));
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            var deviceInfomationService = new DeviceInfomationServiceBuilder(bluetoothManager).Build();
            
            bluetoothManager.GattSever.AddService(new DeviceInfomationServiceBuilder(bluetoothManager).Build());
            GyroscopeServiceWrapper = new GyroscopeServiceWrapper(bluetoothManager);
            ClientRfcommServiceWrapper = new ClientRfcommServiceWrapper(bluetoothManager);
            
            bluetoothManager.GattSever.AddService(ClientRfcommServiceWrapper.GattServerService);
            bluetoothManager.GattSever.AddService(new BatteryServiceWrapper(bluetoothManager).GattServerService);
            //bluetoothManager.GattSever.AddService(GyroscopeServiceWrapper.GattServerService);
            bluetoothManager.GattSever.StartAdvertising();
            ClientRfcommServiceWrapper.OnRfcommAddressWrite += ClientRfcommServiceWrapper_OnRfcommAddressWrite;

            Sketch = new Sketch();
            Sketch.SkiaManager.Init(CanvasView.InvalidateSurface, SKMatrix.MakeScale(1, -1));
            
            
            Sketch.SketchEngine.Instantiate<GridRenderer>();
            Sketch.SketchEngine.Instantiate<PointerInfoBoard>();
            Sketch.SkiaManager.BeforePaint += SkiaManager_BeforePaint;
            ManagerManager = DependencyService.Get<IManagerManager>();
            SketchInputManager = Sketch.SketchEngine.Instantiate<SketchInputManager>();
            Sketch.Start();
            SketchInputManager.Init(ManagerManager.InputManager);
            var joystick = Sketch.SketchEngine.Instantiate<ColorJoystick>();
            joystick.RectTransform.AnchorMax = new Vector2(1, 1);
            joystick.RectTransform.AnchorMin = new Vector2(0, 0);
            joystick.RectTransform.OffsetMax = new Vector2(-200, -200);
            joystick.RectTransform.OffsetMin = new Vector2(200, 200);
            joystick.Level = 2;
            ExampleSketchObject = Sketch.SketchEngine.Instantiate<ExampleSketchObject>();
            CanvasView.InvalidateSurface();
            ExampleSketchObject.Position = new SKPoint(Sketch.Width/2, Sketch.Height/2);
            SensorSpeed speed = SensorSpeed.Fastest;
            
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            Gyroscope.Start(speed);

            Device.StartTimer(TimeSpan.FromSeconds(1 / 60f), () => { CanvasView.InvalidateSurface(); return !true; });
        }

        private async void ClientRfcommServiceWrapper_OnRfcommAddressWrite(object sender, KeyValuePair<Bluetooth.IBluetoothDevice, ulong> e)
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            var device = bluetoothManager.GetBluetoothDevice(e.Value);
            await device.RfcommConnectAsync();
            var serviceResult = await device.GetRfcommServicesAsync();
            if(serviceResult.Error != Bluetooth.Rfcomm.BluetoothError.Success)
            {
                return;
            }
            IRfcommDeviceService gyroService = null;
            foreach(var service in serviceResult.Services)
            {
                if(service.ServiceId == Constant.GyroscopeServiceGuid)
                {
                    gyroService = service;
                }
            }
            if(gyroService == null)
            {
                return;
            }
            await gyroService.ConnectAsync();
            GyroConnection = gyroService.RfcommConnection;
        }

        private DateTime _PreviousReadDateTime = DateTime.Now;
        private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var timeSpan = (DateTime.Now - _PreviousReadDateTime).TotalMilliseconds;
            if(timeSpan < 1000.0/40)
            {
                return;
            }
            _PreviousReadDateTime = DateTime.Now;
            var delta = new SKPoint(e.Reading.AngularVelocity.X*10, e.Reading.AngularVelocity.Y*10);
            ExampleSketchObject.Position = ExampleSketchObject.Position + delta;
            GyroscopeServiceWrapper.UpdateAngularVelocity(e.Reading.AngularVelocity);
        }

        private void SkiaManager_BeforePaint(object sender, SKCanvas e)
        {
            var skiaManager = sender as SkiaManager;
            SKMatrix.MakeTranslation(0, e.LocalClipBounds.Height);
            var matrix = skiaManager.SketchSpaceToCanvasSpaceMatrix;
            Sketch.Width = e.LocalClipBounds.Width;
            Sketch.Height = e.LocalClipBounds.Height;
            
            SKPoint sketchSize = new SKPoint(Sketch.Width, Sketch.Height);
            
            //matrix.SetScaleTranslate(1f, -1f, e.LocalClipBounds.Width / 2, e.LocalClipBounds.Height / 2);
            var factor = e.LocalClipBounds.Width / sketchSize.X;
            matrix.SetScaleTranslate(e.LocalClipBounds.Width/sketchSize.X, -e.LocalClipBounds.Height/sketchSize.Y, 0, e.LocalClipBounds.Height);
            skiaManager.SketchSpaceToCanvasSpaceMatrix = matrix;
            
            //float baseDpi = 96;
            Matrix3x2 epxToPx = Matrix3x2.CreateScale(1);
            Matrix3x2 pxToSketchSpace = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(0, -e.LocalClipBounds.Height), Matrix3x2.CreateScale(sketchSize.X/e.LocalClipBounds.Width, -sketchSize.Y / e.LocalClipBounds.Height));
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
