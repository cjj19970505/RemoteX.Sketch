using RemoteX.Bluetooth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteX.Sketch.DesktopControl
{
    class MacAddressUlongToStringConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UInt64 macAddress = (UInt64)value;
            return BluetoothUtils.AddressInt64ToString(macAddress);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
