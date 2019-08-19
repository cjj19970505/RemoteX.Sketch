using RemoteX.Bluetooth;
using RemoteX.Bluetooth.LE.Gatt.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace RemoteX.Sketch.UwpExample
{
    public sealed partial class BleDeviceSelectorDialog : ContentDialog
    {
        ObservableCollection<BleDeviceViewModel> ScanResultItemSource;
        public IBluetoothManager BluetoothManager { get; }
        public Guid TargetServiceGuid { get; }

        public IGattClientService Result { get; private set; }
        public BleDeviceSelectorDialog(IBluetoothManager bluetoothManager, Guid targetServiceGuid)
        {
            ScanResultItemSource = new ObservableCollection<BleDeviceViewModel>();
            BluetoothManager = bluetoothManager;
            TargetServiceGuid = targetServiceGuid;
            this.InitializeComponent();
            ScanResultListView.ItemsSource = ScanResultItemSource;
            ConnectButton.Click += ConnectButton_Click;
            Opened += BleDeviceSelectorDialog_Opened;
            Closed += BleDeviceSelectorDialog_Closed;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            IsPrimaryButtonEnabled = false;
            var selectedDevice = (ScanResultListView.SelectedItem as BleDeviceViewModel).BluetoothDevice;
            MessageTextBlock.Text = "正在连接到:" + selectedDevice.Name;
            await selectedDevice.GattClient.ConnectToServerAsync();
            GattServiceResult serviceResult = new GattServiceResult();
            try
            {
                serviceResult = await selectedDevice.GattClient.DiscoverAllPrimaryServiceAsync();
            }
            catch(Exception exception)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MessageTextBlock.Text = "异常：" + exception.Message;
                    IsPrimaryButtonEnabled = true;
                });
                return;
            }
            
            if (serviceResult.ProtocolError != Bluetooth.LE.Gatt.GattErrorCode.Success)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MessageTextBlock.Text = "Protocol Error:" + serviceResult.ProtocolError;
                    IsPrimaryButtonEnabled = true;
                });
                return;
            }
            IGattClientService clientService = null;
            foreach (var service in serviceResult.Services)
            {
                if (service.Uuid == TargetServiceGuid)
                {
                    clientService = service;
                }
            }
            if (clientService == null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MessageTextBlock.Text = "该设备中没有对应的服务";
                    IsPrimaryButtonEnabled = true;
                });
                return;
            }
            Result = clientService;
            
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Hide();
                IsPrimaryButtonEnabled = true;
            });
        }

        private void BleDeviceSelectorDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            BluetoothManager.LEScanner.Added -= LEScanner_Added;
            BluetoothManager.LEScanner.Removed -= LEScanner_Removed;
            BluetoothManager.LEScanner.EnumerationCompleted -= LEScanner_EnumerationCompleted;
            BluetoothManager.LEScanner.Stopped -= LEScanner_Stopped;

            if (BluetoothManager.LEScanner.Status == Bluetooth.LE.BluetoothLEScannerState.Started || BluetoothManager.LEScanner.Status == Bluetooth.LE.BluetoothLEScannerState.EnumerationCompleted)
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
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ScanResultItemSource.Clear();
            });
            
            BluetoothManager.LEScanner.Start();
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

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
            
            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
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
