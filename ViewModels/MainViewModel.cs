﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models;
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

        private UserControl currentPage;
        public UserControl CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        private void CreateUserModel(ObservableCollection<UserModel> users)
        {
            Users = users;
        }

        public MainViewModel()
        {
            _context = new ServerContext();

            _context.GetRequest<ObservableCollection<UserModel>>("User", CreateUserModel);

            //LoginView page1 = new LoginView();
            ChatMenuView page1 = new ChatMenuView();
            CurrentPage = page1;
        }

        private Command getUser;
        public Command SwitchToPage2Command
        {
            get
            {
                return getUser ?? (getUser = new Command(obj =>
                {
                    User = (UserModel)obj;
                    ChatMenuView page2 = new ChatMenuView();
                    CurrentPage = page2;
                    //try
                    //{
                    //    // подключемся к хабу
                    //    connection.StartAsync();
                    //    MessageBox.Show("Вы вошли в чат");
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(ex.Message);
                    //}
                }));
            }
        }
    }
}
