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

namespace RemoteX.Sketch.XamExapmple
{
    class BioshockGamePad:SketchPage
    {
        AreaJoystick<VirtualKeyCode> LeftStick;
        AreaJoystick<VirtualKeyCode> DirectionPad;
        TouchpadJoystick RightStick;
        ControllerButton XButton;
        ControllerButton YButton;
        ControllerButton AButton;
        ControllerButton BButton;

        public KeyboardServiceWrapper KeyboardServiceWrapper { get; private set; }
        public GyroscopeRfcommServiceConnectionWrapper GyroscopeRfcommServiceConnectionWrapper { get; private set; }

        public BioshockGamePad(IInputManager inputManager) : base(inputManager)
        {

        }
        protected override void Setup()
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            var deviceInfomationService = new DeviceInfomationServiceBuilder(bluetoothManager).Build();

            bluetoothManager.GattSever.AddService(new DeviceInfomationServiceBuilder(bluetoothManager).Build());
            bluetoothManager.GattSever.AddService(new RfcommServerServiceWrapper(bluetoothManager).GattServerService);
            KeyboardServiceWrapper = new KeyboardServiceWrapper(bluetoothManager);
            bluetoothManager.GattSever.AddService(KeyboardServiceWrapper.GattServerService);
            bluetoothManager.GattSever.StartAdvertising();

            var createServiceProviderTask = bluetoothManager.CreateRfcommServiceProviderAsync(GyroscopeRfcommServiceConnectionWrapper.RfcommServiceId);
            createServiceProviderTask.Wait();
            var gyroRfcommServiceProvider = createServiceProviderTask.Result;
            gyroRfcommServiceProvider.OnConnectionReceived += GyroRfcommServiceProvider_OnConnectionReceived;
            gyroRfcommServiceProvider.StartAdvertising();

            XButton = Sketch.SketchEngine.Instantiate<ControllerButton>();
            XButton.ButtonString = "X";
            YButton = Sketch.SketchEngine.Instantiate<ControllerButton>();
            YButton.ButtonString = "Y";
            AButton = Sketch.SketchEngine.Instantiate<ControllerButton>();
            AButton.ButtonString = "A";
            BButton = Sketch.SketchEngine.Instantiate<ControllerButton>();
            BButton.ButtonString = "B";
            XButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            XButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            XButton.RectTransform.OffsetMin = new Vector2(1178.85f, -285.65f);
            XButton.RectTransform.OffsetMax = new Vector2(1295.15f, -169.35f);
            YButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            YButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            YButton.RectTransform.OffsetMin = new Vector2(1295.15f, -169.35f);
            YButton.RectTransform.OffsetMax = new Vector2(1411.45f, -53.05001f);
            BButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            BButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            BButton.RectTransform.OffsetMin = new Vector2(1411.45f, -285.65f);
            BButton.RectTransform.OffsetMax = new Vector2(1527.75f, -169.35f);
            AButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            AButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            AButton.RectTransform.OffsetMin = new Vector2(1295.15f, -401.95f);
            AButton.RectTransform.OffsetMax = new Vector2(1411.45f, -285.65f);


            LeftStick = Sketch.SketchEngine.Instantiate<LineAreaJoystick<VirtualKeyCode>>();
            LeftStick.OnAreaStatusChanged += LeftStick_OnAreaStatusChanged;
            LeftStick.MaxLength = 300;
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_D, -60, 60, 0, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_W, 30, 150, 0, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_A, 120, 240, 0, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.VK_S, 210, 330, 0, float.PositiveInfinity));
            LeftStick.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.LSHIFT, 0, 360, 0.7f, float.PositiveInfinity));
            LeftStick.RectTransform.AnchorMax = new Vector2(0.00f, 1.00f);
            LeftStick.RectTransform.AnchorMin = new Vector2(0.00f, 1.00f);
            LeftStick.RectTransform.OffsetMin = new Vector2(73.00f, -394.60f);
            LeftStick.RectTransform.OffsetMax = new Vector2(423.00f, -44.60f);
            LeftStick.Level = 3;

            DirectionPad = Sketch.SketchEngine.Instantiate<LineAreaJoystick<VirtualKeyCode>>();
            DirectionPad.MaxLength = 100;
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.RIGHT, -60, 60, 0, float.PositiveInfinity));
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.UP, 30, 150, 0, float.PositiveInfinity));
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.LEFT, 120, 240, 0, float.PositiveInfinity));
            DirectionPad.AddArea(AreaJoystick<VirtualKeyCode>.Area<VirtualKeyCode>.CreateFromAngle(VirtualKeyCode.DOWN, 210, 330, 0, float.PositiveInfinity));
            DirectionPad.RectTransform.AnchorMax = new Vector2(0f, 1f);
            DirectionPad.RectTransform.AnchorMin = new Vector2(0f, 1f);
            DirectionPad.RectTransform.OffsetMin = new Vector2(73f, -842f);
            DirectionPad.RectTransform.OffsetMax = new Vector2(423f, -492f);

            RightStick = Sketch.SketchEngine.Instantiate<TouchpadJoystick>();
            RightStick.OnMove += RightStick_OnMove;
            RightStick.RectTransform.AnchorMax = new Vector2(0f, 1f);
            RightStick.RectTransform.AnchorMin = new Vector2(0f, 1f);
            RightStick.RectTransform.OffsetMin = new Vector2(1177.8f, -842f);
            RightStick.RectTransform.OffsetMax = new Vector2(1527.8f, -492f);
            RightStick.Level = 3;
            

            Sketch.SketchEngine.Instantiate<SketchBorderRenderer>();
            Sketch.SketchEngine.Instantiate<RectTransformFrameRenderer>();

            
        }

        private void RightStick_OnMove(object sender, Vector2 e)
        {
            System.Diagnostics.Debug.WriteLine(e);
        }

        private void GyroRfcommServiceProvider_OnConnectionReceived(object sender, IRfcommConnection e)
        {
            GyroscopeRfcommServiceConnectionWrapper = new GyroscopeRfcommServiceConnectionWrapper(e);
        }

        private void LeftStick_OnAreaStatusChanged(object sender, AreaJoystick<VirtualKeyCode>.AreaStatusChangeEventArgs<VirtualKeyCode> e)
        {
        }
    }
}
