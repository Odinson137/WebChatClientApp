using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models;

namespace WebChatClientApp.ViewModels
{
    public class ChatMenuViewModel : BaseViewModel
    {
        private HubConnection connection;

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

                OnPropertyChanged("Chat");
            }
        }



        private ObservableCollection<ChatModel> chats;
        public ObservableCollection<ChatModel> Chats
        {
            get => chats;
            set
            {
                chats = value;
                OnPropertyChanged("Chats");
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

        public ChatMenuViewModel()
        {
            _context = new ServerContext();
            Chats = new ObservableCollection<ChatModel>();
        }

        private async void ChatMenuFunc()
        {
            await _context.GetRequest<ObservableCollection<ChatModel>>("Chat", User.Id, CreateChat);

            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7078/chat")
                .Build();

            connection.On<int, int, string>("OnReceiveMessage", OnReceiveMessage);

            await Connecting();
        }

        public void CreateChat(ObservableCollection<ChatModel> models)
        {
            Chats = models;
            if (Chats.Count > 0)
            {
                Chat = Chats[0];
            }
        }

        private async Task Connecting()
        {
            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("SendMessage", User.Id);
            }
            catch (Exception ex)    
            {
                MessageBox.Show("Connecting error" + ex.Message);
            }
        }

        private async Task OnReceiveMessage(int userSendId, int chatSendId, string sendMessage)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Chat.Messages.Add(new MessageModel()
                {
                    Text = sendMessage
                });
            });
        }

        private async Task GetMessages()
        {
            await _context.GetRequest<ObservableCollection<MessageModel>>("Message", Chat.ChatId, CreateMessage);
        }

        public void CreateMessage(ObservableCollection<MessageModel> messages)
        {
            Chat.Messages = messages;
        }

        private Command loadingCommand;
        public Command LoadingCommand
        {
            get
            {
                return loadingCommand ?? (loadingCommand = new Command(obj =>
                {
                    User = (UserModel)obj;
                    //User = new UserModel() { UserID = "1", UserName = "Yura" };
                     
                    ChatMenuFunc();
                }));
            }
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
                        Users = new ObservableCollection<UserModel>() { user}
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
                    if ((string)obj == "1") {
                        SelectedMenu = 1;
                    } else
                    {
                        SelectedMenu = 0;
                    }
                    OnPropertyChanged("SelectedMenu");
                }));
            }
        }
    }
}
