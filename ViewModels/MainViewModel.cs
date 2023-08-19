using System.Collections.Generic;
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
                OnPropertyChanged("User");
                
                //ChatMenuView page2 = new ChatMenuView();
                //CurrentPage = page2;
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

            User = new UserModel()
            {
                Id = "9df1d71c-bdca-4532-978f-1f7423f0cddd",
                UserName = "Kolobock"
            };

            LoginView page = new LoginView();
            //ChatMenuView page = new ChatMenuView();

            CurrentPage = page;
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
                }));
            }
        }

        private Command switchToOne;
        public Command SwitchToPage1Command
        {
            get
            {
                return switchToOne ?? (switchToOne = new Command(obj =>
                {
                    LoginView page1 = new LoginView();
                    CurrentPage = page1;
                }));
            }
        }
    }
}
