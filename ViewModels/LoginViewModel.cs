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

        private User selectedUser;
        public User SelectedUser
        {
            get
            {
                if (selectedUser != null) 
                    CreateUser = new User { UserName = selectedUser.UserName, Password = selectedUser.Password };
                return selectedUser;
            }
            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
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
            SelectedUser = new User();

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
                SelectedUser = null;
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

        public void GetFailed(object failed)
        {
            FailedPassword = (string)failed;
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
            SelectedUser = null;
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

        public void GetUserLoginId(object response)
        {
            _cryptUser.AddUser(new User { UserName = CreateUser.UserName, Password = CreateUser.Password });
            FailedPassword = "Successfully added";
            Users.Add(new User { UserName = CreateUser.UserName, Password = CreateUser.Password });
            CreateUser = new User();
            SelectedUser = new User();
        }

        private Command deleteUserCommand;
        public Command DeleteUserCommand
        {
            get
            {
                return deleteUserCommand ?? (deleteUserCommand = new Command(obj =>
                {
                    DeleteUser();
                }));
            }
        }

        private async void DeleteUser()
        {
            await _context.DeleteRequestUrl($"User/{CreateUser.UserName}", DeleteUserFunc);
        }

        public void DeleteUserFunc(string message)
        {
            FailedPassword = "Successfully deleted";
            _cryptUser.RemoveUserByUsername(SelectedUser.UserName);
            Users.Remove(SelectedUser);
            CreateUser = new User();
        }

        private Command removeFromListCommand;
        public Command RemoveFromListCommand
        {
            get
            {
                return removeFromListCommand ?? (removeFromListCommand = new Command(obj =>
                {
                    _cryptUser.RemoveUserByUsername(SelectedUser.UserName);
                    Users.Remove(SelectedUser);
                }));
            }
        }

    }
}
