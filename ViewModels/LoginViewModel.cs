using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models;
using WebChatClientApp.Models.DTO;

namespace WebChatClientApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
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

        public User CreateUser { get; set; }

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

        public LoginViewModel()
        {
            _context = new ServerContext();

            _context.GetRequest<ObservableCollection<UserModel>>("User", CreateUserModel);
            CreateUser = new User();
        }

        public delegate void UserViewModelDelegate();

        private void CreateUserModel(ObservableCollection<UserModel> users)
        {
            Users = users;
        }

        private string failedPassword = "";
        public string FailedPassword
        {
            get => failedPassword;
            set
            {
                failedPassword = value;
                OnPropertyChanged("FailedPassword");
            }
        }

        private Command getAuthority;
        public Command GetAuthority
        {
            get
            {
                return getAuthority ?? (getAuthority = new Command(obj =>
                {
                    if (CreateUser.UserName == null || CreateUser.Password == null)
                    {
                        FailedPassword = "Not all data is filled in";
                    } else
                    {
                        _context.PostRequest("User", CreateUser, GetUserId, GetFailed);
                    }
                }));
            }
        }

        public void GetUserId(string userId)
        {
            UserModel user = new UserModel()
            {
                Id = userId,
                UserName = CreateUser.UserName
            };
            Users.Add(user);
            FailedPassword = "Successfully created";
        }

        public void GetFailed(string failed)
        {
            FailedPassword = failed;
        }
    }
}
