using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models;

namespace WebChatClientApp.ViewModels
{
    public class ChatMenuViewModel : BaseViewModel
    {
        private HubConnection connection;
        private readonly ServerContext _context;
        public UserModel User { get; set; }

        private ChatModel chat;
        public ChatModel Chat
        {
            get => chat;
            set
            {
                chat = value;
                OnPropertyChanged("Chat");
            }
        }
        public ObservableCollection<ChatModel> Chats { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ChatMenuViewModel()
        {
            _context = new ServerContext();
            Chat = new ChatModel();
            Chats = new ObservableCollection<ChatModel>();
            Messages = new ObservableCollection<MessageModel>();
        }

        private void ChatMenuFunc()
        {
            Messages.Add(new MessageModel()
            {
                MessageID = 0,
                SendMessage = DateTime.Now,
                Text = "Hello Pidor",
            });

            User = new UserModel() { UserID = 1, Name = "Yura" };

            _context.GetRequest<ObservableCollection<ChatModel>>("ChatUser", User.UserID, CreateChat);

            //connection = new HubConnectionBuilder()
            //    .WithUrl("https://localhost:7078/chat")
            //    .Build();

            ////connection.On<string>("ReceiveUserId", ReceiveUserId);
            //connection.On<int, int, string>("ReceiveMessage", OnReceiveMessage);

            //Connecting();
        }

        public void CreateChat(ObservableCollection<ChatModel> models)
        {
            Chats = models;
            Chat = Chats[0];
            OnPropertyChanged("Chats");
        }

        private void Connecting()
        {
            try
            {
                // подключемся к хабу
                connection.StartAsync();
                MessageBox.Show("Вы вошли в чат");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnReceiveMessage(int userSendId, int chatSendId, string sendMessage)
        {
            //userConnectedId = message;
        }

        private Command loadingCommand;
        public Command LoadingCommand
        {
            get
            {
                return loadingCommand ?? (loadingCommand = new Command(obj =>
                {
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
                    string message = (string)obj;
                    Messages.Add(new MessageModel()
                    {
                        SendMessage = DateTime.Now,
                        Text = message
                    });

                }));
            }
        }
    }
}
