using RemoteX.Sketch.Forms;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using RemoteX.Sketch.InputComponent;
using RemoteX.Input;
using RemoteX.Sketch.Bluetooth;
using RemoteX.Bluetooth.Procedure.Server;
using RemoteX.Bluetooth.LE.Gatt.Server;
using Xamarin.Forms;
using RemoteX.Bluetooth.Rfcomm;
using SkiaSharp;
using Xamarin.Essentials;

namespace RemoteX.Sketch.XamExapmple
{
    class BioshockGamePad:SketchPage
    {
        AreaJoystick<VirtualKeyCode> LeftStick;
        AreaJoystick<VirtualKeyCode> DirectionPad;
        TouchpadJoystick RightStick;
        AreaJoystick<VirtualKeyCode> ButtonStick;
        private DateTime _LatestRightStickMoveDateTime = DateTime.Now;

        public KeyboardServiceWrapper KeyboardServiceWrapper { get; private set; }
        public MouseServiceWrapper MouseServiceWrapper { get; private set; }

        public BioshockGamePad(IInputManager inputManager) : base(inputManager)
        {

        }
        protected override void Setup()
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            var deviceInfomationService = new DeviceInfomationServiceBuilder(bluetoothManager).Build();

            SensorSpeed speed = SensorSpeed.Game;
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            Gyroscope.Start(speed);

            bluetoothManager.GattSever.AddService(new DeviceInfomationServiceBuilder(bluetoothManager).Build());
            bluetoothManager.GattSever.AddService(new RfcommServerServiceWrapper(bluetoothManager).GattServerService);
            KeyboardServiceWrapper = new KeyboardServiceWrapper(bluetoothManager);
            bluetoothManager.GattSever.AddService(KeyboardServiceWrapper.GattServerService);
            bluetoothManager.GattSever.StartAdvertising();

            var createServiceProviderTask = bluetoothManager.CreateRfcommServiceProviderAsync(MouseServiceWrapper.ServiceId);
            createServiceProviderTask.Wait();
            var mouseServiceProvider = createServiceProviderTask.Result;
            mouseServiceProvider.OnConnectionReceived += MouseServiceProvider_OnConnectionReceived; ;
            mouseServiceProvider.StartAdvertising();

            var keyManager = DependencyService.Get<IManagerManager>().KeyManager;
            keyManager.VolumnDownKeyDown += KeyManager_VolumnDownKeyDown;
            keyManager.VolumnDownKeyUp += KeyManager_VolumnDownKeyUp;
            keyManager.VolumnUpKeyDown += KeyManager_VolumnUpKeyDown;
            keyManager.VolumnUpKeyUp += KeyManager_VolumnUpKeyUp;
            ButtonStick = Sketch.SketchEngine.Instantiate<LineAreaJoystick<VirtualKeyCode>>();
            ButtonStick.OnAreaStatusChanged += Stick_OnAreaStatusChanged;
            ButtonStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.LSHIFT, -60, 60, 0, float.PositiveInfinity));
            ButtonStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_V, 30, 150, 0, float.PositiveInfinity));
            ButtonStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_R, 120, 240, 0, float.PositiveInfinity));
            ButtonStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.SPACE, 210, 330, 0, float.PositiveInfinity));
            ButtonStick.RectTransform.AnchorMax = new Vector2(0f, 1f);
            ButtonStick.RectTransform.AnchorMin = new Vector2(0f, 1f);
            ButtonStick.RectTransform.OffsetMin = new Vector2(1178.8f, -428f);
            ButtonStick.RectTransform.OffsetMax = new Vector2(1528.8f, -78f);


            LeftStick = Sketch.SketchEngine.Instantiate<LineAreaJoystick<VirtualKeyCode>>();
            LeftStick.OnAreaStatusChanged += Stick_OnAreaStatusChanged;
            LeftStick.MaxLength = 120;
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_D, -60, 60, 0.3f, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_W, 30, 150, 0.3f, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_A, 120, 240, 0.3f, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_S, 210, 330, 0.3f, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.LCONTROL, 0, 360, 1f, float.PositiveInfinity));
            LeftStick.RectTransform.AnchorMax = new Vector2(0f, 1f);
            LeftStick.RectTransform.AnchorMin = new Vector2(0f, 1f);
            LeftStick.RectTransform.OffsetMin = new Vector2(26f, -457f);
            LeftStick.RectTransform.OffsetMax = new Vector2(376f, -107f);
            LeftStick.Level = 3;

            DirectionPad = Sketch.SketchEngine.Instantiate<LineAreaJoystick<VirtualKeyCode>>();
            DirectionPad.OnAreaStatusChanged += Stick_OnAreaStatusChanged;
            DirectionPad.MaxLength = 100;
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_Z, -60, 60, 0, float.PositiveInfinity));
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_Q, 30, 150, 0, float.PositiveInfinity));
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_E, 120, 240, 0, float.PositiveInfinity));
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_N, 210, 330, 0, float.PositiveInfinity));
            DirectionPad.RectTransform.AnchorMax = new Vector2(0f, 1f);
            DirectionPad.RectTransform.AnchorMin = new Vector2(0f, 1f);
            DirectionPad.RectTransform.OffsetMin = new Vector2(26f, -807f);
            DirectionPad.RectTransform.OffsetMax = new Vector2(376f, -457f);

            RightStick = Sketch.SketchEngine.Instantiate<TouchpadJoystick>();
            RightStick.OnMove += RightStick_OnMove;
            RightStick.RectTransform.AnchorMax = new Vector2(0f, 1f);
            RightStick.RectTransform.AnchorMin = new Vector2(0f, 1f);
            RightStick.RectTransform.OffsetMin = new Vector2(1178.8f, -778f);
            RightStick.RectTransform.OffsetMax = new Vector2(1528.8f, -428f);
            RightStick.Level = 3;
            Sketch.SketchEngine.Instantiate<SketchBorderRenderer>();
            Sketch.SketchEngine.Instantiate<RectTransformFrameRenderer>();
            

        }

        private void KeyManager_VolumnUpKeyUp(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(0, 1);
            e.Handle = true;
        }

        private void KeyManager_VolumnUpKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(0, 0);
            e.Handle = true;
        }

        private void KeyManager_VolumnDownKeyUp(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(1, 1);
            e.Handle = true;
        }

        private void KeyManager_VolumnDownKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(1, 0);
            e.Handle = true;
        }

        private void MouseServiceProvider_OnConnectionReceived(object sender, IRfcommConnection e)
        {
            MouseServiceWrapper = new MouseServiceWrapper(e);
        }
        
        private async void RightStick_OnMove(object sender, Vector2 e)
        {
            if (MouseServiceWrapper == null)
            {
                return;
            }
            _LatestRightStickMoveDateTime = DateTime.Now;
            var moveAmount = -e*0.15f;
            await MouseServiceWrapper.MoveMouseAsync(moveAmount);
        }

        private void Stick_OnAreaStatusChanged(object sender, AreaJoystick<VirtualKeyCode>.AreaStatusChangeEventArgs<VirtualKeyCode> e)
        {
            KeyboardServiceWrapper.UpdateKeyStatus(e.Area.AreaIdentifier, (KeyStatus)e.NewStatus);
        }

        private async void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            if (MouseServiceWrapper == null)
            {
                return;
            }
            if (DateTime.Now - _LatestRightStickMoveDateTime < TimeSpan.FromMilliseconds(30))
            {
                return;
            }
            var moveAmount = new Vector2(e.Reading.AngularVelocity.X, e.Reading.AngularVelocity.Y);
            await MouseServiceWrapper.MoveMouseAsync(moveAmount);
        }
    }
}
