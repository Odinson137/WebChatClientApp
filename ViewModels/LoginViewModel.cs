using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WebChatClientApp.Commands;
using WebChatClientApp.Data;
using WebChatClientApp.Models.DTO;
using WebChatClientApp.Services;

namespace WebChatClientApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private ServerContext _context;
        private UserCredentialsService _cryptUser;

        private User createUser;
        public User CreateUser
        {
            get => createUser;
            set
            {
                createUser = value;
                OnPropertyChanged(nameof(CreateUser));
            }
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
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

            CreateUser = new User();

            _cryptUser = new UserCredentialsService();
            Users = new ObservableCollection<User>(_cryptUser.GetUsers());
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
                        _context.PostRequest("User/Registr", CreateUser, GetUserLoginId, GetFailed);
                    }
                }));
            }
        }

        public void GetFailed(string failed)
        {
            FailedPassword = failed;
        }


        private Command getLogin;
        public Command GetLogin
        {
            get
            {
                return getLogin ?? (getLogin = new Command(obj =>
                {
                    GetLoginFunc();
                }));
            }
        }

        private async void GetLoginFunc()
        {
            //_cryptUser.RemoveUserByUsername(_cryptUser.GetUsers().Select(x => x.UserName).FirstOrDefault());
            if (CreateUser.UserName == null || CreateUser.Password == null)
            {
                FailedPassword = "Not all data is filled in";
                return;
            }

            if (Users.Select(user => user.UserName).Contains(CreateUser.UserName))
            {
                FailedPassword = "There is already such a user";
                return;
            }

            await _context.PostRequest("User/Login", CreateUser, GetUserLoginId, GetFailed);
            
        }

        public void GetUserLoginId(string userId)
        {
            _cryptUser.AddUser(CreateUser);
            FailedPassword = "Successfully added";
            Users.Add(CreateUser);

        }
    }
}
