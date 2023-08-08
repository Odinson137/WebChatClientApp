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
    public class ParameterService
    {
        public string Page1Parameter { get; set; }
        public int Page2Parameter { get; set; }
    }



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
                //SelectedViewModel = new ChatMenuViewModel();
                //SelectedViewModel.Parameter = user;
                //OnPropertyChanged("SelectedViewModel");
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


        private void CreateUserModel(ICollection<UserModel> users)
        {
            Users = new ObservableCollection<UserModel>(users);
        }

        private string page1Parameter;
        private int page2Parameter;

        public string Page1Parameter
        {
            get { return page1Parameter; }
            set
            {
                page1Parameter = value;
                OnPropertyChanged(nameof(Page1Parameter));
            }
        }
        public ParameterService ParameterService { get; } = new ParameterService();
        public MainViewModel()
        {
            //SwitchToPage1Command = new Command(SwitchToPage1);
            //SwitchToPage2Command = new Command(SwitchToPage2);

            _context = new ServerContext();

            _context.GetRequest<UserModel>("User", CreateUserModel);

            LoginView page1 = new LoginView();
            //ParameterService.Page1Parameter = "yura";
            //page1.Parameter = SwitchToPage2;
            CurrentPage = page1;
            //SelectedViewModel = new BaseViewModel();
        }

        //// Команда для переключения на страницу 1
        //public ICommand SwitchToPage1Command { get; }

        //// Команда для переключения на страницу 2
        //public ICommand SwitchToPage2Command { get; }

        private Command getUser;
        public Command SwitchToPage2Command
        {
            get
            {
                return getUser ?? (getUser = new Command(obj =>
                {
                    User = (UserModel)obj;
                    SwitchToPage2();
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

        private void SwitchToPage1()
        {
            LoginView page1 = new LoginView();
            //page1.Parameter = "Some data for Page 1"; // Установка параметра для страницы 1
            CurrentPage = page1;
        }

        private void SwitchToPage2()
        {
            ChatMenuView page2 = new ChatMenuView();
            //page2.Parameter = User; // Установка параметра для страницы 2
            CurrentPage = page2;
        }
    }
}
