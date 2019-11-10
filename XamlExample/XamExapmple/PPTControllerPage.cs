using RemoteX.Bluetooth.LE.Gatt.Server;
using RemoteX.Bluetooth.Procedure.Server;
using RemoteX.Bluetooth.Rfcomm;
using RemoteX.Input;
using RemoteX.Sketch.Bluetooth;
using RemoteX.Sketch.Forms;
using RemoteX.Sketch.InputComponent;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RemoteX.Sketch.XamExapmple
{
    class PPTControllerPage:SketchPage
    {
        public KeyboardServiceWrapper KeyboardServiceWrapper { get; private set; }
        public MouseServiceWrapper MouseServiceWrapper { get; private set; }
        public PPTControllerPage(IInputManager inputManager):base(inputManager)
        {
            SketchSize = new Vector2(900, 1600);
        }

        public KeyboardKeyButton LeftMouseButton;
        public KeyboardKeyButton RightMouseButton;
        public KeyboardKeyButton StopMouseButton;
        public TouchpadJoystick MouseStick;
        public KeyboardKeyButton OpenLazerButton;
        private bool _StopMouse;
        protected override void Setup()
        {
            base.Setup();
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

            LeftMouseButton = Sketch.SketchEngine.Instantiate<KeyboardKeyButton>();
            LeftMouseButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            LeftMouseButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            LeftMouseButton.RectTransform.OffsetMin = new Vector2(53f, -1157f);
            LeftMouseButton.RectTransform.OffsetMax = new Vector2(403f, -807f);
            LeftMouseButton.KeyDown += LeftMouseButton_KeyDown;
            LeftMouseButton.KeyUp += LeftMouseButton_KeyUp;

            RightMouseButton = Sketch.SketchEngine.Instantiate<KeyboardKeyButton>();
            RightMouseButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            RightMouseButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            RightMouseButton.RectTransform.OffsetMin = new Vector2(53f, -1552f);
            RightMouseButton.RectTransform.OffsetMax = new Vector2(403f, -1202f);
            RightMouseButton.KeyDown += RightMouseButton_KeyDown;
            RightMouseButton.KeyUp += RightMouseButton_KeyUp;

            StopMouseButton = Sketch.SketchEngine.Instantiate<KeyboardKeyButton>();
            StopMouseButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            StopMouseButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            StopMouseButton.RectTransform.OffsetMin = new Vector2(53f, -778f);
            StopMouseButton.RectTransform.OffsetMax = new Vector2(214.6f, -616.4f);
            StopMouseButton.KeyDown += StopMouseButton_KeyDown;
            StopMouseButton.KeyUp += StopMouseButton_KeyUp;

            MouseStick = Sketch.SketchEngine.Instantiate<TouchpadJoystick>();
            MouseStick.RectTransform.AnchorMax = new Vector2(0f, 1f);
            MouseStick.RectTransform.AnchorMin = new Vector2(0f, 1f);
            MouseStick.RectTransform.OffsetMin = new Vector2(500f, -1359f);
            MouseStick.RectTransform.OffsetMax = new Vector2(850f, -1009f);
            MouseStick.OnMove += MouseStick_OnMove;

            OpenLazerButton = Sketch.SketchEngine.Instantiate<KeyboardKeyButton>();
            OpenLazerButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            OpenLazerButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            OpenLazerButton.RectTransform.OffsetMin = new Vector2(86f, -286.2f);
            OpenLazerButton.RectTransform.OffsetMax = new Vector2(436f, -78.00001f);
            OpenLazerButton.KeyDown += OpenLazerButton_KeyDown;
            OpenLazerButton.KeyUp += OpenLazerButton_KeyUp;
            Sketch.SketchEngine.Instantiate<SketchBorderRenderer>();
            Sketch.SketchEngine.Instantiate<RectTransformFrameRenderer>();



        }

        private void OpenLazerButton_KeyUp(object sender, EventArgs e)
        {
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.LCONTROL, KeyStatus.Released);
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.VK_L, KeyStatus.Released);
        }

        private void OpenLazerButton_KeyDown(object sender, EventArgs e)
        {
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.LCONTROL, KeyStatus.Pressed);
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.VK_L, KeyStatus.Pressed);

            

        }

        private async void MouseStick_OnMove(object sender, Vector2 e)
        {
            var moveAmount = -e * 0.15f;
            await MouseServiceWrapper.MoveMouseAsync(moveAmount);
        }

        private void StopMouseButton_KeyUp(object sender, EventArgs e)
        {
            _StopMouse = false;
        }

        private void StopMouseButton_KeyDown(object sender, EventArgs e)
        {
            _StopMouse = true;
        }

        private void RightMouseButton_KeyUp(object sender, EventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(1, 1);
        }

        private void RightMouseButton_KeyDown(object sender, EventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(1, 0);
        }

        private void LeftMouseButton_KeyUp(object sender, EventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(0, 1);
        }

        private void LeftMouseButton_KeyDown(object sender, EventArgs e)
        {
            KeyboardServiceWrapper.UpdateMouseStatus(0, 0);
        }

        private async void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            if (MouseServiceWrapper == null)
            {
                return;
            }
            if(_StopMouse)
            {
                return;
            }
            var moveAmount = new Vector2(e.Reading.AngularVelocity.Z, -e.Reading.AngularVelocity.X);
            await MouseServiceWrapper.MoveMouseAsync(moveAmount);
        }

        private void KeyManager_VolumnUpKeyUp(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.PRIOR, KeyStatus.Released);
            e.Handle = true;
        }

        private void KeyManager_VolumnUpKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.PRIOR, KeyStatus.Pressed);
            e.Handle = true;
        }

        private void KeyManager_VolumnDownKeyUp(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.NEXT, KeyStatus.Released);
            e.Handle = true;
        }

        private void KeyManager_VolumnDownKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.NEXT, KeyStatus.Pressed);
            e.Handle = true;
        }

        private void MouseServiceProvider_OnConnectionReceived(object sender, IRfcommConnection e)
        {
            MouseServiceWrapper = new MouseServiceWrapper(e);
        }
    }
}
