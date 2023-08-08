using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            MessageBox.Show("dsfsdf");
        }

        public ChatMenuViewModel(string aaa)
        {
            MessageBox.Show(aaa);
        }

        //public ChatMenuViewModel(UserModel _user)
        //{
        //    _context = new ServerContext();

        //    User = _user;
        //    //MessageBox.Show(_user.Name);
        //}
    }
}
