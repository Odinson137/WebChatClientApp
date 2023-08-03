using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebChatClientApp.Models;
using System.Windows.Controls;

namespace WebChatClientApp.Data
{

    // класс, который создает дополнительный слой абстракции для работы с запросами к серверу.
    // Каждый ViewModel, которому будет необходим доступ к серверу, будет должен создать экземпляр этого класса
    public class ServerContext<T>
    {
        const string url = "https://localhost:7078";
        public ServerContext()
        {
            
        }

        // Метод для создания запросов к серверу
        // Должен иметь реализацию для всех случаев
        public async Task CreateRequest(string id)
        {
            string apiUrl = $"{url}/api/User/{id}";
            try
            {
                await SendRequestToServer(apiUrl);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        // Делегат, который передает значения модели со стороны сервера из данного класса в тот класс, который его реализует
        // Позволяет реализующим классас абсрагироваться от http запросов
        private delegate void CreateModel();

        // Метод для получения результов от сервера по запросу
        // Должен принимать делегат для записи значений из серверных моделей в модели на стороне клиента
        private async Task SendRequestToServer(string apiUrl)
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
                    T result = JsonConvert.DeserializeObject<T>(responseBody);
                    //UserName.Text = result.Name;
                    //UserLastName.Text = result.LastName;
                    //UserPassword.Text = result.Password;
                    //return result;
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Ошибка при отправке запроса: " + ex.Message);
                    throw new Exception("Ошибка при отправке запроса: " + ex.Message);
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Ошибка при обработке JSON: " + ex.Message);
                    throw new Exception("Ошибка при обработке JSON: " + ex.Message);
                }
            }
        }

    }
}
