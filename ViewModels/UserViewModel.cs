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
    public class UserViewModel
    {
        private ServerContext<UserModel> _context;

        public UserModel User { get; set; }
        public ICollection<UserModel> Users { get; set; } // временно
        public UserViewModel()
        {
            //_context = new ServerContext<UserModel>();
        }

        private void CreateUserModel()
        {
            User = new UserModel()
            {
                //Name = 
            };
        }


        private Command clickMouse;
        public Command ClickMouse
        {
            get 
            {
                return clickMouse ?? (clickMouse = new Command(obj =>
                {
                    MessageBox.Show("as");
                }));
            }
            set
            {
                clickMouse.CanExecute(true);
            }
        }
    }
}
