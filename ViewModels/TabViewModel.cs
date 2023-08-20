using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models;

namespace WebChatClientApp.ViewModels
{
    public class TabViewModel : INotifyPropertyChanged
    {
        private readonly ServerContext _context;

        private UserModel user;
        public UserModel User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged(nameof(user));
            }
        }

        private UserModel addUser;
        public UserModel AddUser
        {
            get => addUser;
            set
            {
                addUser = value;
                OnPropertyChanged(nameof(AddUser));
            }
        }


        private ChatModel chat;
        public ChatModel Chat
        {
            get => chat;
            set
            {
                chat = value;

                if (chat != null && chat.Messages == null)
                {
                    GetMessages();
                }

                OnPropertyChanged(nameof(TabTitle));
                OnPropertyChanged(nameof(Chat));
            }
        }

        private ObservableCollection<ChatModel> chats;
        public ObservableCollection<ChatModel> Chats
        {
            get => chats;
            set
            {
                chats = value;
                OnPropertyChanged(nameof(Chats));
            }
        }

        private ObservableCollection<string> friends;
        public ObservableCollection<string> Friends
        {
            get
            {
                return friends;
            }
            set
            {
                friends = value;
                OnPropertyChanged("Friends");
            }
        }

        public string TabTitle
        {
            get
            {
                if (Chat != null && Chat.Title != null && Chat.Title != "")
                {
                    return Chat.Title;
                } else
                {
                    return "New Tab";
                }
            }
            set
            {
                OnPropertyChanged(nameof(TabTitle));
            }
        }

        public int SelectedTabIndex { get; set; }
        public TabViewModel(ServerContext context, ObservableCollection<ChatModel> chats, UserModel user, int index)
        {
            _context = context;

            Chats = chats;
            if (Chats.Count > 0)
                Chat = Chats[0];

            User = user;
            AddUser = new UserModel();

            SelectedTabIndex = index;
        }

        private async Task GetMessages()
        {
            await _context.GetRequest<ObservableCollection<MessageModel>>("Message", Chat.ChatId, CreateMessage);
        }
        public void CreateMessage(ObservableCollection<MessageModel> messages)
        {
            Chat.Messages = messages;
        }

        private Command sendMessage;
        public Command SendMessage
        {
            get
            {
                return sendMessage ?? (sendMessage = new Command(obj =>
                {
                    TextBox box = (TextBox)obj;
                    string text = box.Text;
                    box.Clear();

                    MessageModel newMessage = new MessageModel()
                    {
                        Text = text,
                        SendTime = DateTime.Now,
                        Id = User.Id,
                        ChatId = Chat.ChatId
                    };

                    Chat.Messages.Add(newMessage);
                    _context.PostRequest("Message", newMessage);
                }));
            }
        }

        private Command createNewChat;
        public Command CreateNewChat
        {
            get
            {
                return createNewChat ?? (createNewChat = new Command(obj =>
                {
                    string title = (string)obj;

                    if (Chats.All(c => c.Title != title) == false)
                    {
                        MessageBox.Show("Чат с таким названием уже существует");
                        return;
                    }

                    ChatModel newChat = new ChatModel()
                    {
                        Title = title,
                        Users = new ObservableCollection<UserModel>() { user }
                    };

                    Chats.Add(newChat);

                    _context.PostRequestUrl("Chat", new Dictionary<string, object>()
                    {
                        ["title"] = title,
                        ["userId"] = user.Id
                    }, GetId);
                }));
            }
        }

        public void GetId(string chatId)
        {
            int id = int.Parse(chatId);
            Chat = Chats.Last();
            Chat.ChatId = id;
        }

        private Command renameChat;
        public Command RenameChat
        {
            get
            {
                return renameChat ?? (renameChat = new Command(obj =>
                {
                    string newTitle = (string)obj;

                    Chat.Title = newTitle;

                    _context.PutRequestUrl("Chat", new Dictionary<string, object>()
                    {
                        ["title"] = newTitle,
                        ["chatId"] = chat.ChatId
                    });
                }));
            }
        }

        private Command deleteChat;
        public Command DeleteChat
        {
            get
            {
                return deleteChat ?? (deleteChat = new Command(obj =>
                {
                    _context.DeleteRequestUrl("Chat", Chat.ChatId);
                    Chats.Remove(chat);
                }));
            }
        }

        public int SelectedMenu { get; set; } = 0;
        private Command changeMenu;
        public Command ChangeMenu
        {
            get
            {
                return changeMenu ?? (changeMenu = new Command(obj =>
                {
                    if ((string)obj == "1")
                    {
                        SelectedMenu = 1;
                    }
                    else
                    {
                        SelectedMenu = 0;
                    }
                    OnPropertyChanged("SelectedMenu");
                }));
            }
        }

        private Command addUserToChat;
        public Command AddUserToChat
        {
            get
            {
                return addUserToChat ?? (addUserToChat = new Command(obj =>
                {
                    _context.PostRequestUrl($"Chat/{Chat.ChatId}/{AddUser.UserName}", SuccessfullyPostSend, FailPostSend);
                }));
            }
        }

        public void SuccessfullyPostSend(string message)
        {
            MessageBox.Show(message);
        }

        public void FailPostSend(string error)
        {
            MessageBox.Show(error);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
