using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models;
using WebChatClientApp.Models.DTO;
using WebChatClientApp.Views;

namespace WebChatClientApp.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private ServerContext _context;

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


        private BaseViewModel currentPage;
        public BaseViewModel CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        public MainViewModel()
        {
            _context = new ServerContext();

            var page = new LoginViewModel();
            CurrentPage = page;
        }

        private Command getUser;
        public Command SwitchToPage2Command
        {
            get
            {
                return getUser ?? (getUser = new Command(obj =>
                {
                    if (obj == null)
                        return;

                    SwitchPage((User)obj);
                }));
            }
        }

        private async void SwitchPage(User user)
        {
            if (user.UserName == null || user.Password == null)
                return;
            User = new UserModel() { UserName = user.UserName };
            await _context.PostRequest("User/Login", user, GetUserLoginId, GetFailed, true);
            ChatMenuViewModel page2 = new ChatMenuViewModel();

            CurrentPage = page2;
            CurrentPage.UserMain = User;

            var sharedData = MultyPagesSingleton.Instance;
            sharedData.SetModel(User);
            sharedData.Context = _context;
        }

        public void GetUserLoginId(object responseContent)
        {
            //JObject jsonResponse = JObject.Parse(responseContent);

            //string token = jsonResponse["token"].ToString();
            //string userId = Convert.ToString(jsonResponse["id"]);

            User.Id = (string)responseContent;
            //User.Token = token;
        }

        public void GetFailed(object responseContent)
        {
            MessageBox.Show("Up the server, please");
            Application.Current.Shutdown();
        }


        private Command switchToOne;
        public Command SwitchToPage1Command
        {
            get
            {
                return switchToOne ?? (switchToOne = new Command(obj =>
                {
                    var connection = (HubConnection)obj;
                    connection.StopAsync();
                    LoginViewModel page1 = new LoginViewModel();
                    CurrentPage = page1;
                }));
            }
        }
    }
}
