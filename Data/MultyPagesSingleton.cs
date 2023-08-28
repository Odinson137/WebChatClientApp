using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChatClientApp.Models;

namespace WebChatClientApp.Data
{
    public class MultyPagesSingleton
    {
        private static MultyPagesSingleton _instance;
        private string _sharedValue;
        public UserModel UserModel { get; set; }
        public ServerContext Context { get; set; }
        private MultyPagesSingleton()
        {
            _sharedValue = "Initial Value";
        }

        public void SetModel(UserModel user)
        {
            UserModel = user;
        }

        public UserModel GetModel()
        {
            return UserModel;
        }

        public static MultyPagesSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MultyPagesSingleton();
                }
                return _instance;
            }
        }

        public string SharedValue
        {
            get { return _sharedValue; }
            set { _sharedValue = value; }
        }
    }

}
