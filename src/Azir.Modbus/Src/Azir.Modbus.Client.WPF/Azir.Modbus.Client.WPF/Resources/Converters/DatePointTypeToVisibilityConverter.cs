using System;
using System.Windows;
using System.Windows.Data;
using Azir.Modbus.Protocol.DataPoints;

namespace Azir.Modbus.Client.WPF.Resources.Converters
{
    public class DatePointTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dataPointType = (DataPointType)value;
            if (dataPointType == DataPointType.WriteAndReadByFunNum01
                || dataPointType == DataPointType.WriteAndReadByFunNum03)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
