using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models;
using static WebChatClientApp.ViewModels.UserViewModel;

namespace WebChatClientApp.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private ServerContext _context;

        private readonly HubConnection connection;

        private UserModel user;
        public UserModel User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged("User");
            }
        }

        private string page1Parameter;
        public string Page1Parameter
        {
            get { return page1Parameter; }
            set
            {
                page1Parameter = value;
                OnPropertyChanged(nameof(Page1Parameter));
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
        // временно

        public UserViewModel()
        {
            _context = new ServerContext();

            _context.GetRequest<UserModel>("User", CreateUserModel);

            // создаем подключение к хабу
            //connection = new HubConnectionBuilder()
            //    .WithUrl("https://localhost:7078/chat")
            //    .Build();

            ////connection.On<string>("ReceiveUserId", ReceiveUserId);
            //connection.On<string>("ReceiveMessage", OnReceiveMessage);
        }

        public delegate void UserViewModelDelegate();

        private void CreateUserModel(ICollection<UserModel> users)
        {
            Users = new ObservableCollection<UserModel>(users);
        }

        //private void ReceiveUserId(string message)
        //{
        //    userConnectedId = message;
        //}

        private void OnReceiveMessage(string message)
        {
            MessageBox.Show(message);
        }



        //public void AddUserModel(UserModel model)
        //{
        //    User = model;
        //}

        //private Command getUser;
        //public Command GetUser
        //{
        //    get 
        //    {
        //        return getUser ?? (getUser = new Command(obj =>
        //        {
        //            try
        //            {
        //                // подключемся к хабу
        //                connection.StartAsync();
        //                MessageBox.Show("Вы вошли в чат");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //            }
        //        }));
        //    }
        //}

        //public void GetUsers(ICollection<UserModel> models)
        //{
        //    Users = new ObservableCollection<UserModel>(models);
        //}

        //private Command getUsers;
        //public Command GetAllUsers
        //{
        //    get
        //    {
        //        return getUsers ?? (getUsers = new Command(obj =>
        //        {
        //            SendMessageAsync("asdasd");
        //        }));
        //    }
        //}

        //private async Task SendMessageAsync(string message)
        //{
        //    try
        //    {
        //        await connection.SendAsync("Send", "asd");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка отправки сообщения: {ex.Message}");
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //public void OnPropertyChanged([CallerMemberName] string prop = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        //}
    }
}
