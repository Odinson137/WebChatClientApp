using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Globalization;
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
}
