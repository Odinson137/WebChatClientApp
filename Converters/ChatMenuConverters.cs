using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WebChatClientApp.Converters
{
    class ActiveConnection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connection = (HubConnection)value;
            if (connection != null)
                if (connection.State == HubConnectionState.Connected)
                    return "Green";
            return "Red";

        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class TextAlignment : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //MessageBox.Show(values[0].ToString());
            if (values[0].ToString() == values[1].ToString())
                return HorizontalAlignment.Right;
            return HorizontalAlignment.Left;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("HH:mm");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
