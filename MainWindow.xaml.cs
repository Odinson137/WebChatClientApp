using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using Newtonsoft.Json;
//using Newtonsoft.Json;

namespace WebChatClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    class User
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }



    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task<User> SendRequestToServer(string apiUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    User result = JsonConvert.DeserializeObject<User>(responseBody);
                    UserName.Text = result.Name;
                    UserLastName.Text = result.LastName;
                    UserPassword.Text = result.Password;
                    return result;
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Ошибка при отправке запроса: " + ex.Message);
                    //throw new Exception("Ошибка при отправке запроса: " + ex.Message);
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Ошибка при обработке JSON: " + ex.Message);
                }
            }
            return null;
        }

        private async void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string id = TextBoxId.Text;
            string apiUrl = $"https://localhost:7078/api/User/{id}";
            MessageBox.Show("Asfd");
            try
            {
                User response = await SendRequestToServer(apiUrl);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }
    }
}
