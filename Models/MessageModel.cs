using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebChatClientApp.Models
{
    public class MessageModel : INotifyPropertyChanged
    {
        public int MessageID { get; set; }
        public int UserID { get; set; }
        public int ChatID { get; set; }

        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }
        public DateTime SendTime { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
