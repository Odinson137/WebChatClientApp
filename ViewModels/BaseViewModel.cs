using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebChatClientApp.Commands;
using WebChatClientApp.Models;

namespace WebChatClientApp.ViewModels
{    
    public class BaseViewModel : INotifyPropertyChanged
    {
        public UserModel UserMain { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
