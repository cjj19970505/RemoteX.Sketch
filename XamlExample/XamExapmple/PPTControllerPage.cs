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
        public KeyboardKeyButton DrawAndPressButton;
        public Quaternion LatestOrientation;
        public Quaternion MouseUseOrientationBase;
        private bool _MouseMoving;
        private bool _LaserOn;
        private bool Laser
        {
            get
            {
                return _LaserOn;
            }
            set
            {
                if (value == _LaserOn)
                {
                    return;
                }
                if (value)
                {
                    Pen = false;
                }
                _LaserOn = !_LaserOn;
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.LCONTROL, KeyStatus.Pressed);
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.VK_L, KeyStatus.Pressed);
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.LCONTROL, KeyStatus.Released);
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.VK_L, KeyStatus.Released);
            }
        }
        private bool _PenOn;
        private bool Pen
        {
            get
            {
                return _PenOn;
            }
            set
            {
                if (value == _PenOn)
                {
                    return;
                }
                if(value)
                {
                    Laser = false;
                }
                _PenOn = !_PenOn;
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.LCONTROL, KeyStatus.Pressed);
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.VK_P, KeyStatus.Pressed);
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.LCONTROL, KeyStatus.Released);
                KeyboardServiceWrapper.UpdateKeyStatus(VirtualKeyCode.VK_P, KeyStatus.Released);
            }
        }
        protected override void Setup()
        {
            base.Setup();
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            var deviceInfomationService = new DeviceInfomationServiceBuilder(bluetoothManager).Build();

            SensorSpeed speed = SensorSpeed.Game;
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            Gyroscope.Start(speed);
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            OrientationSensor.Start(speed);

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

            DrawAndPressButton = Sketch.SketchEngine.Instantiate<KeyboardKeyButton>();
            DrawAndPressButton.RectTransform.AnchorMax = new Vector2(0f, 1f);
            DrawAndPressButton.RectTransform.AnchorMin = new Vector2(0f, 1f);
            DrawAndPressButton.RectTransform.OffsetMin = new Vector2(688.4f, -778f);
            DrawAndPressButton.RectTransform.OffsetMax = new Vector2(850f, -616.4f);
            DrawAndPressButton.KeyDown += DrawAndPressButton_KeyDown;
            DrawAndPressButton.KeyUp += DrawAndPressButton_KeyUp;

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
            _MouseMoving = false;

        }

        private void DrawAndPressButton_KeyUp(object sender, EventArgs e)
        {
            Pen = false;
            Laser = true;
            KeyboardServiceWrapper.UpdateMouseStatus(0, 1);
        }

        private void DrawAndPressButton_KeyDown(object sender, EventArgs e)
        {
            if(!_MouseMoving)
            {
                return;
            }
            Pen = true;
            KeyboardServiceWrapper.UpdateMouseStatus(0, 0);
        }

        private void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            LatestOrientation = e.Reading.Orientation;
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
            _MouseMoving = false;
            Laser = false;
            Pen = false;
        }

        private void StopMouseButton_KeyDown(object sender, EventArgs e)
        {
            _MouseMoving = true;
            MouseUseOrientationBase = LatestOrientation;
            Laser = true;
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
            if(!_MouseMoving)
            {
                return;
            }
            var angularVelocity = e.Reading.AngularVelocity;
            var angluarVelocityQuaternion = AngluarVelocityToQuaternion(angularVelocity);
            var angluarVelocityMat = Matrix4x4.CreateFromQuaternion(angluarVelocityQuaternion);

            //var matrix = Matrix4x4.Transpose(Matrix4x4.CreateFromQuaternion(LatestOrientation));
            var deviceToWorldMat = Matrix4x4.Transpose(Matrix4x4.CreateFromQuaternion(LatestOrientation));
            var angluarVelocityMatWolrdspace = Matrix4x4.Multiply(angluarVelocityMat, deviceToWorldMat);


            //var moveAmount = new Vector2(transformedAngularVelocity.Z, transformedAngularVelocity.X);
            var moveAmount = new Vector2(angularVelocity.Z, -angularVelocity.X);
            //var moveAmount = new Vector2(Vector3.Transform(Vector3.UnitX, angluarVelocityQuaternion).Z, -Vector3.Transform(Vector3.UnitX, angluarVelocityQuaternion).X);
            await MouseServiceWrapper.MoveMouseAsync(moveAmount);
        }

        private Quaternion AngluarVelocityToQuaternion(Vector3 angluarVelocity)
        {
            var angle = angluarVelocity.Length();
            Quaternion q = new Quaternion();
            q.X = (float)(angluarVelocity.X * Math.Sin(angle / 2) / angle);
            q.Y = (float)(angluarVelocity.Y * Math.Sin(angle / 2) / angle);
            q.Z = (float)(angluarVelocity.Z * Math.Sin(angle / 2) / angle);
            q.W = (float)Math.Cos(angle/2);
            return q;
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
