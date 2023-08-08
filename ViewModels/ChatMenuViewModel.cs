using System;
using System.Collections.Generic;
using System.Linq;
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
        private ServerContext _context;

        //private readonly UserModel user;
        public UserModel User { get; set; }
        public ChatMenuViewModel()
        {
            
        }

        public ChatMenuViewModel(string aaa)
        {
            MessageBox.Show(aaa);

        }

        private Command getUser1;
        public Command Command
        {
            get
            {
                return getUser1 ?? (getUser1 = new Command(obj =>
                {
                    User = (UserModel)obj;

                    MessageBox.Show(User.Name);
                    //User = (UserModel)obj;
                    //SwitchToPage2();
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

        //public ChatMenuViewModel(UserModel _user)
        //{
        //    _context = new ServerContext();

        //    User = _user;
        //    //MessageBox.Show(_user.Name);
        //}
    }
}
