using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;


namespace WebChatClientApp.Data
{

    // Класс, который создает дополнительный слой абстракции для работы с запросами к серверу.
    // Каждый ViewModel, которому будет необходим доступ к серверу, должен создать экземпляр этого класса
    public class ServerContext<T>
    {
        const string url = "https://localhost:7078";
        public ServerContext() {}

        // Метод для создания запросов к серверу
        // Должен иметь реализацию для всех случаев
        //
        // Для получения моделей из базы данных
        // adress: ../Controller/
        public async void CreateRequest(string controller, GetModel getModel)
        {
            string apiUrl = $"{url}/api/{controller}";
            try
            {
                await SendRequestToServer(apiUrl, getModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        public async void CreateRequest(string controller, string id, GetModel getModel)
        {
            string apiUrl = $"{url}/api/{controller}/{id}";
            try
            {
                await SendRequestToServer(apiUrl, getModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        // Для получения коллекций из базы данных
        // adress: ../Controller/id
        public async void CreateRequest(string controller, GetCollectinModel getCollectinModel)
        {
            string apiUrl = $"{url}/api/{controller}";
            try
            {
                await SendRequestToServer(apiUrl, getCollectinModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        public async void CreateRequest(string controller, string id, GetCollectinModel getCollectinModel)
        {
            string apiUrl = $"{url}/api/{controller}/{id}";
            try
            {
                await SendRequestToServer(apiUrl, getCollectinModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        // Делегат, который передает значения модели со стороны сервера из данного класса в тот класс, который его реализует
        // Позволяет реализующим классам абсрагироваться от http запросов
        public delegate void GetModel(T model);
        public delegate void GetCollectinModel(ICollection<T> model);

        // Метод для получения результов от сервера по запросу
        // Имеет две реализации для моделей и для коллекций
        //
        // Должен принимать делегат для записи значений из серверных моделей в модели на стороне клиента
        private async Task SendRequestToServer(string apiUrl, GetModel getModel)
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
                    getModel(result);
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

        // Должен принимать делегат для записи значений из серверных коллекций в коллекции на стороне клиента
        private async Task SendRequestToServer(string apiUrl, GetCollectinModel getCollectionModel)
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
                    ICollection<T> result = JsonConvert.DeserializeObject<ICollection<T>>(responseBody);
                    //foreach ()
                    getCollectionModel(result);

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
