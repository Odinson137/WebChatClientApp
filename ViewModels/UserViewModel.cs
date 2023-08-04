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
    public class UserViewModel : INotifyPropertyChanged
    {
        private ServerContext<UserModel> _context;

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
            _context = new ServerContext<UserModel>();
        }

        public void AddUserModel(UserModel model)
        {
            User = model;
        }

        private Command getUser;
        public Command GetUser
        {
            get 
            {
                return getUser ?? (getUser = new Command(obj =>
                {
                    _context.CreateRequest("User", (string)obj, AddUserModel);
                }));
            }
        }

        public void GetUsers(ICollection<UserModel> models)
        {
            Users = new ObservableCollection<UserModel>(models);
        }

        private Command getUsers;
        public Command GetAllUsers
        {
            get
            {
                return getUsers ?? (getUsers = new Command(obj =>
                {
                    _context.CreateRequest("User", GetUsers);
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
