using RemoteX.Bluetooth;
using RemoteX.Bluetooth.LE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RemoteX.Bluetooth.Procedure.Client;
using RemoteX.Bluetooth.Rfcomm;

namespace RemoteX.Sketch.DesktopControl
{
    /// <summary>
    /// BleDeviceSelectorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BleDeviceSelectorWindow : Window
    {
        public IBluetoothManager BluetoothManager { get; }
        ObservableCollection<BleDeviceViewModel> ScanResultItemSource;
        public ConnectionProfile ConnectionProfile { get; }
        public IBluetoothDevice SelectedDevice { get; private set; }
        public ConnectionBuildResult ConnectionBuildResult { get; private set; }
        public BleDeviceSelectorWindow(IBluetoothManager bluetoothManager, ConnectionProfile connectionProfile)
        {
            BluetoothManager = bluetoothManager;
            ConnectionProfile = connectionProfile;
            ScanResultItemSource = new ObservableCollection<BleDeviceViewModel>();
            InitializeComponent();
            DeviceListView.ItemsSource = ScanResultItemSource;
            IsVisibleChanged += BleDeviceSelectorWindow_IsVisibleChanged;
        }

        private void BleDeviceSelectorWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                ScanResultItemSource.Clear();
                BluetoothManager.LEScanner.Added += LEScanner_Added;
                BluetoothManager.LEScanner.Removed += LEScanner_Removed;
                BluetoothManager.LEScanner.Start();
            }
            else
            {
                BluetoothManager.LEScanner.Added -= LEScanner_Added;
                BluetoothManager.LEScanner.Removed -= LEScanner_Removed;
                if(BluetoothManager.LEScanner.Status == BluetoothLEScannerState.Started)
                {
                    BluetoothManager.LEScanner.Stop();
                }
            }
        }

        private async void LEScanner_Removed(object sender, IBluetoothDevice e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                var removedViewModel = _GetViewModelFromDevice(e);
                ScanResultItemSource.Remove(removedViewModel);
            });
        }

        private async void LEScanner_Added(object sender, IBluetoothDevice e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                var exsitedViewModel = _GetViewModelFromDevice(e);
                if (exsitedViewModel == null)
                {
                    ScanResultItemSource.Add(new BleDeviceViewModel(e));
                }
            });
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if(BluetoothManager.LEScanner.Status == BluetoothLEScannerState.Started)
            {
                BluetoothManager.LEScanner.Stop();
            }
            if(BluetoothManager.RfcommScanner.Status == BluetoothRfcommScannerState.Started)
            {
                BluetoothManager.RfcommScanner.Stop();
            }
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(BluetoothManager, ConnectionProfile, SelectedDevice);
            try
            {
                var result = await connectionBuilder.StartAsync();
                ConnectionBuildResult = result;
                await Dispatcher.InvokeAsync(() =>
                {
                    Close();
                });
            }
            catch(Exception exception)
            {
                BluetoothManager.LEScanner.Start();
                await Dispatcher.InvokeAsync(() =>
                {
                    MsgTextBlock.Text = exception.Message;
                });
            }
        }

        private BleDeviceViewModel _GetViewModelFromDevice(IBluetoothDevice bluetoothDevice)
        {
            foreach (var viewmodel in ScanResultItemSource)
            {
                if (viewmodel.BluetoothDevice.Address == bluetoothDevice.Address)
                {
                    return viewmodel;
                }
            }
            return null;
        }

        private void DeviceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                SelectedDevice = (e.AddedItems[0] as BleDeviceViewModel).BluetoothDevice;
                ConnectButton.IsEnabled = true;
            }
            else
            {
                SelectedDevice = null;
                ConnectButton.IsEnabled = false;
            }
            
        }
    }

    public class BleDeviceViewModel
    {
        public IBluetoothDevice BluetoothDevice { get; }
        public string Name
        {
            get
            {
                return BluetoothDevice.Name;
            }
        }
        public UInt64 Address
        {
            get
            {
                return BluetoothDevice.Address;
            }
        }
        public BleDeviceViewModel(IBluetoothDevice device)
        {
            BluetoothDevice = device;
        }
    }
}
