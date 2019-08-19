using RemoteX.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RemoteX.Sketch.UwpExample
{
    class MacAddressUlongToStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            UInt64 macAddress = (UInt64)value;
            return BluetoothUtils.AddressInt64ToString(macAddress);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return BluetoothUtils.AddressStringToInt64(targetType.ToString());
            }
            catch (Exception)
            {
                return 0ul;
            }
        }
    }
}
