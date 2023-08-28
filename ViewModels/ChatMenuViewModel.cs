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
using WebChatClientApp.Models.DTO;
using static WebChatClientApp.Data.ServerContext;

namespace WebChatClientApp.ViewModels
{
    public class ChatMenuViewModel : BaseViewModel
    {
        private HubConnection connection;
        public HubConnection GetConnection
        {
            get => connection;
            set
            {
                OnPropertyChanged(nameof(GetConnection));
            }
        }

        public ServerContext _context;

        private ObservableCollection<TabViewModel> tabs = new ObservableCollection<TabViewModel>();
        public ObservableCollection<TabViewModel> Tabs
        {
            get { return tabs; }
            set
            {
                tabs = value;
                OnPropertyChanged(nameof(Tabs));
            }
        }

        private ObservableCollection<string> titleTabs = new ObservableCollection<string>();
        public ObservableCollection<string> TitleTabs
        {
            get { return titleTabs; }
            set
            {
                titleTabs = value;
                OnPropertyChanged(nameof(TitleTabs));
            }
        }

        private TabViewModel selectedTab;
        public TabViewModel SelectedTab
        {
            get { return selectedTab; }
            set
            {
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

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
            //_context = new ServerContext();
            Chats = new ObservableCollection<ChatModel>();
        }

        private async void ChatMenuFunc()
        {
            var sharedData = MultyPagesSingleton.Instance;
            User = sharedData.GetModel();
            _context = sharedData.Context;

            await _context.GetRequest<ObservableCollection<ChatModel>>($"Chat/{User.Id}", CreateChat);

            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7078/chat")
                .Build();

            connection.On<string, int, string>("OnReceiveMessage", OnReceiveMessage);
            connection.On<int, string, ICollection<UserModel>>("OnReceiveInvitation", OnReceiveInvitation);

            await Connecting();

            AddTabFunc("New Tab");
        }

        public void CreateChat(ObservableCollection<ChatModel> models)
        {
            Chats = models;
        }

        public void CloseApp(string message)
        {
            Application.Current.Shutdown();
        }

        private async Task Connecting()
        {
            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("SendMessage", User.Id);
                GetConnection = connection;
            }
            catch (Exception ex)    
            {
                MessageBox.Show("Connecting error" + ex.Message);
            }
        }

        private async Task OnReceiveMessage(string userSendId, int chatSendId, string sendMessage)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var chat = Chats.Where(chat => chat.ChatId == chatSendId).First();
                chat.Messages.Add(new MessageModel()
                {
                    Id = userSendId,
                    Text = sendMessage,
                    SendTime = DateTime.Now
                });
            });
        }

        private async Task OnReceiveInvitation(int chatId, string title, ICollection<UserModel> users)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var chat = new ChatModel() { ChatId = chatId, Title = title, Users = new ObservableCollection<UserModel>(users) };
                var beChat = Chats.Where(x => x.ChatId == chatId).FirstOrDefault();
                if (beChat != null)
                {
                    beChat.Users = chat.Users;
                }
                else
                {
                    Chats.Add(chat);
                }
            });
        }

        private Command changeTab;
        public Command ChangeTab
        {
            get
            {
                return changeTab ?? (changeTab = new Command(obj =>
                {
                    int index = (int)obj;
                    
                    SelectedTab = Tabs[index];
                }));
            }
        }

        private int index = 0;
        private Command addTab;
        public Command AddTab
        {
            get
            {
                return addTab ?? (addTab = new Command(obj =>
                {
                    if (Tabs.Count() < 6)
                    {
                        AddTabFunc("New Tab");
                    }
                }));
            }
        }

        private void AddTabFunc(string title)
        {
            var newTab = new TabViewModel(_context, Chats, User, index++);
            Tabs.Add(newTab);
            SelectedTab = newTab;
        }

        private Command loadingCommand;
        public Command LoadingCommand
        {
            get
            {
                return loadingCommand ?? (loadingCommand = new Command(obj =>
                {
                    User = (UserModel)obj;
                    ChatMenuFunc();
                }));
            }
        }


    }
}
