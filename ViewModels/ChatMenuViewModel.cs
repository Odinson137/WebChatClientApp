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

                if (chat.Messages == null)
                {
                    GetMessages();
                    chat.Message = new MessageModel();
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
        
        public ChatMenuViewModel()
        {
            _context = new ServerContext();
            //Chat = new ChatModel();
            Chats = new ObservableCollection<ChatModel>();
            //Chat.Messages = new ObservableCollection<MessageModel>();
        }

        private void ChatMenuFunc()
        {
            _context.GetRequest<ObservableCollection<ChatModel>>("Chat", User.UserID, CreateChat);

            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7078/chat")
                .Build();

            connection.On<int, int, string>("OnReceiveMessage", OnReceiveMessage);

            Connecting();
        }

        public void CreateChat(ObservableCollection<ChatModel> models)
        {
            Chats = models;
            if (Chats.Count > 0)
            {
                Chat = Chats[0];
            }
        }

        private async void Connecting()
        {
            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("SendMessage", User.UserID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connecting error" + ex.Message);
            }
        }

        private void OnReceiveMessage(int userSendId, int chatSendId, string sendMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Chat.Messages.Add(new MessageModel()
                {
                    Text = sendMessage
                });
            });
        }

        private void GetMessages()
        {
            _context.GetRequest<ObservableCollection<MessageModel>>("Message", Chat.ChatID, CreateMessage);
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
                    //User = (UserModel)obj;
                    User = new UserModel() { UserID = 1, Name = "Yura", LastName = "Bury", Password = "123" };
                     
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
                    
                    //MessageModel message = new MessageModel()
                    //{
                    //    UserID = User.UserID,
                    //    ChatID = Chat.ChatID,
                    //    Text = text
                    //};
                    Chat.Messages.Add(Chat.Message);
                    Chat.Message = new MessageModel();
                    //Chat.Message.Text = "";

                    //_context.PostRequest("Message", message);
                }));
            }
        }
    }
}
