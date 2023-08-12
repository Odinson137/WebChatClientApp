using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WebChatClientApp.Models
{
    public class ChatModel : INotifyPropertyChanged
    {
        public int ChatID { get; set; }
        public string Title { get; set; }

        private MessageModel message;
        public MessageModel Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private ObservableCollection<UserModel> users;
        public ObservableCollection<UserModel> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }

        private ObservableCollection<MessageModel> messages;
        public ObservableCollection<MessageModel> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged("Messages");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
