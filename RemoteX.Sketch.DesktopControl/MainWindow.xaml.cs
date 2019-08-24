using AutoHotkey.Interop;
using RemoteX.Bluetooth.Procedure.Client;
using RemoteX.Bluetooth.Win10;
using RemoteX.Sketch.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput;

namespace RemoteX.Sketch.DesktopControl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Guid GyroscopeReadingUuid;
        Guid KeyboardServiceUuid;
        Guid KeyboardCharacteristicUuid;
        Guid MouseServiceUuid;
        Guid MouseCharacteristicUuid;
        ConnectionBuildResult ConnectionBuildResult;
        GyroscopeRfcommServiceConnectionWrapper GyroscopeRfcommServiceConnectionWrapper;
        MouseServiceWrapper MouseServiceWrapper;
        KeyboardServiceClientWrapper KeyboardServiceClientWrapper;
        AutoHotkeyEngine Ahk { get; }
        InputSimulator InputSimulator;
        KeyActionManager KeyActionManager;
        public MainWindow()
        {
            InitializeComponent();
            Ahk = AutoHotkeyEngine.Instance;
            InputSimulator = new InputSimulator();
            KeyActionManager = new KeyActionManager(Dispatcher, InputSimulator);
            BluetoothManager bluetoothManager = new BluetoothManager();
            GyroscopeReadingUuid = GyroscopeRfcommServiceConnectionWrapper.RfcommServiceId;
            KeyboardServiceUuid = Constants.KeyboardServiceGuid;
            MouseServiceUuid = Constants.MouseServiceId;
            MouseCharacteristicUuid = Constants.MouseActionCharacteristicWrapper;
            KeyboardCharacteristicUuid = Constants.KeyActionCharacteristicWrapper;

            var characteristicDict = new Dictionary<Guid, List<CharacteristicProfile>>();
            characteristicDict.Add(KeyboardServiceWrapper.Guid, new List<CharacteristicProfile>()
            {
                new CharacteristicProfile
                {
                    Notified = true,
                    Guid = Constants.KeyActionCharacteristicWrapper
                },
                new CharacteristicProfile
                {
                    Notified = true,
                    Guid = Constants.MouseActionCharacteristicWrapper
                }
            });
            var serviceId = new List<Guid>
            {
                MouseServiceWrapper.ServiceId
            };
            ConnectionProfile profile = new ConnectionProfile()
            {
                RequiredCharacteristicGuids = characteristicDict,
                RequiredServiceGuids = serviceId
            };
            
            BleDeviceSelectorWindow bleDeviceSelectorWindow = new BleDeviceSelectorWindow(bluetoothManager, profile);
            bleDeviceSelectorWindow.ShowDialog();
            ConnectionBuildResult = bleDeviceSelectorWindow.ConnectionBuildResult;
            MouseServiceWrapper = new MouseServiceWrapper(ConnectionBuildResult[MouseServiceUuid].RfcommConnection);
            MouseServiceWrapper.OnMouseMoveReceived += MouseServiceWrapper_OnMouseMoveReceived;
            KeyboardServiceClientWrapper = new KeyboardServiceClientWrapper(ConnectionBuildResult[KeyboardServiceUuid, KeyboardCharacteristicUuid], ConnectionBuildResult[KeyboardServiceUuid, MouseCharacteristicUuid]);
            KeyboardServiceClientWrapper.OnKeyStatusChanged += KeyboardServiceClientWrapper_OnKeyStatusChanged;
            KeyboardServiceClientWrapper.OnMouseStatusChanged += KeyboardServiceClientWrapper_OnMouseStatusChanged;
        }

        private void KeyboardServiceClientWrapper_OnMouseStatusChanged(object sender, KeyboardServiceClientWrapper.MouseStatusChangeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.MouseButton + ", " + e.NewStatus);
            if(e.MouseButton == 0)
            {
                if(e.NewStatus == 0)
                {
                    InputSimulator.Mouse.LeftButtonDown();
                }
                else
                {
                    InputSimulator.Mouse.LeftButtonUp();
                }
            }
            else if(e.MouseButton == 1)
            {
                if (e.NewStatus == 0)
                {
                    InputSimulator.Mouse.RightButtonDown();
                }
                else
                {
                    InputSimulator.Mouse.RightButtonUp();
                }
            }
        }

        private void MouseServiceWrapper_OnMouseMoveReceived(object sender, Vector2 e)
        {
            int dX = -(int)(e.X * 20);
            int dY = (int)(e.Y * 20);
            InputSimulator.Mouse.MoveMouseBy(dX, dY);
        }

        private void KeyboardServiceClientWrapper_OnKeyStatusChanged(object sender, KeyboardServiceClientWrapper.KeyStatusChangeEventArgs e)
        {
            if(e.NewStatus == KeyStatus.Pressed)
            {
                KeyActionManager.KeyDown(e.KeyCode);
            }
            else
            {
                KeyActionManager.KeyUp(e.KeyCode);
            }
            
        }

        public List<double> TimeSpans = new List<double>();
        private void GyroscopeRfcommServiceConnectionWrapper_OnReadingUpdated(object sender, Vector3 e)
        {
            int dX = -(int)(e.X * 20);
            int dY = (int)(e.Y * 20);
            InputSimulator.Mouse.MoveMouseBy(dX, dY);
        }
        
    }
}
