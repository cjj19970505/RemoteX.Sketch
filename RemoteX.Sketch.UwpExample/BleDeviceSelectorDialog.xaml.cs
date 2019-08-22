using RemoteX.Bluetooth;
using RemoteX.Bluetooth.LE;
using RemoteX.Bluetooth.LE.Gatt.Client;
using RemoteX.Bluetooth.Procedure.Client;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace RemoteX.Sketch.UwpExample
{
    public sealed partial class BleDeviceSelectorDialog : ContentDialog
    {
        ObservableCollection<BleDeviceViewModel> ScanResultItemSource;
        
        public IBluetoothManager BluetoothManager { get; }
        public ConnectionProfile ConnectionProfile { get; }

        public ConnectionBuildResult Result { get; private set; }
        public BleDeviceSelectorDialog(IBluetoothManager bluetoothManager, ConnectionProfile connectionProfile)
        {
            ScanResultItemSource = new ObservableCollection<BleDeviceViewModel>();
            BluetoothManager = bluetoothManager;
            ConnectionProfile = connectionProfile;
            this.InitializeComponent();
            ScanResultListView.ItemsSource = ScanResultItemSource;
            ConnectButton.Click += ConnectButton_Click;
            Opened += BleDeviceSelectorDialog_Opened;
            Closed += BleDeviceSelectorDialog_Closed;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if(BluetoothManager.LEScanner.Status == BluetoothLEScannerState.Started)
            {
                BluetoothManager.LEScanner.Stop();
            }
            IsPrimaryButtonEnabled = false;
            
            var selectedDevice = (ScanResultListView.SelectedItem as BleDeviceViewModel).BluetoothDevice;
            MessageTextBlock.Text = "正在连接到:" + selectedDevice.Name;
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(BluetoothManager, ConnectionProfile, selectedDevice);
            try
            {
                var connectionBuildResult = await connectionBuilder.StartAsync();
                Result = connectionBuildResult;
                Hide();
            }
            catch(Exception exception)
            {
                MessageTextBlock.Text = exception.Message;
                BluetoothManager.LEScanner.Start();
            }

        }

        private void BleDeviceSelectorDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            BluetoothManager.LEScanner.Added -= LEScanner_Added;
            BluetoothManager.LEScanner.Removed -= LEScanner_Removed;
            BluetoothManager.LEScanner.EnumerationCompleted -= LEScanner_EnumerationCompleted;
            BluetoothManager.LEScanner.Stopped -= LEScanner_Stopped;

            if (BluetoothManager.LEScanner.Status == BluetoothLEScannerState.Started || BluetoothManager.LEScanner.Status == BluetoothLEScannerState.EnumerationCompleted)
            {
                BluetoothManager.LEScanner.Stop();
            }
        }

        private void BleDeviceSelectorDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            Result = null;
            BluetoothManager.LEScanner.Added += LEScanner_Added;
            BluetoothManager.LEScanner.Removed += LEScanner_Removed;
            BluetoothManager.LEScanner.EnumerationCompleted += LEScanner_EnumerationCompleted;
            BluetoothManager.LEScanner.Stopped += LEScanner_Stopped;
            BluetoothManager.LEScanner.Start();
        }

        private async void LEScanner_Stopped(object sender, EventArgs e)
        {
            /*
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ScanResultItemSource.Clear();
            });
            
            BluetoothManager.LEScanner.Start();
            */
        }

        private void LEScanner_EnumerationCompleted(object sender, EventArgs e)
        {
            BluetoothManager.LEScanner.Stop();
        }

        private async void LEScanner_Removed(object sender, IBluetoothDevice e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var removedViewModel = _GetViewModelFromDevice(e);
                ScanResultItemSource.Remove(removedViewModel);
            });
        }

        private async void LEScanner_Added(object sender, IBluetoothDevice e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var exsitedViewModel = _GetViewModelFromDevice(e);
                if(exsitedViewModel == null)
                {
                    ScanResultItemSource.Add(new BleDeviceViewModel(e));
                }
            });
        }
        private BleDeviceViewModel _GetViewModelFromDevice(IBluetoothDevice bluetoothDevice)
        {
            foreach(var viewmodel in ScanResultItemSource)
            {
                if(viewmodel.BluetoothDevice.Address == bluetoothDevice.Address)
                {
                    return viewmodel;
                }
            }
            return null;
        }
    }
}
