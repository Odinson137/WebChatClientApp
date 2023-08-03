using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatClientApp.Models;

namespace WebChatClientApp.Data.Interfaces
{
    public interface IUserRepository
    {
        public ICollection<UserModel> GetUsers();
        public UserModel GetUser(int userId);
        public UserModel GetUser(string Name);
    }
}
