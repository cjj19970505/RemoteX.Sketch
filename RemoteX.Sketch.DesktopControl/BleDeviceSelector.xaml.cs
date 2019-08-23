using RemoteX.Bluetooth;
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

namespace RemoteX.Sketch.DesktopControl
{
    /// <summary>
    /// BleDeviceSelector.xaml 的交互逻辑
    /// </summary>
    public partial class BleDeviceSelector : Window
    {
        public IBluetoothManager BluetoothManager { get; }
        public ObservableCollection<IBluetoothDevice> DeviceListItemSource;
        public BleDeviceSelector(IBluetoothManager bluetoothManager)
        {
            BluetoothManager = bluetoothManager;
            DeviceListItemSource = new ObservableCollection<IBluetoothDevice>();
            InitializeComponent();
            IsVisibleChanged += BleDeviceSelector_IsVisibleChanged;
            BleDeviceListView.ItemsSource = DeviceListItemSource;
        }

        private void BleDeviceSelector_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                BluetoothManager.LEScanner.Added += LEScanner_Added;
                BluetoothManager.LEScanner.Removed += LEScanner_Removed;
                BluetoothManager.LEScanner.EnumerationCompleted += LEScanner_EnumerationCompleted;
                BluetoothManager.LEScanner.Start();
            }
        }

        private void LEScanner_EnumerationCompleted(object sender, EventArgs e)
        {
            BluetoothManager.LEScanner.Stop();
        }

        private async void LEScanner_Removed(object sender, IBluetoothDevice e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DeviceListItemSource.Add(e);
            });
            
        }

        private async void LEScanner_Added(object sender, IBluetoothDevice e)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DeviceListItemSource.Remove(e);
            });
        }
    }
}
