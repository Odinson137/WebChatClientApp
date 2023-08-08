using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Text;

namespace WebChatClientApp.Data
{

    // Класс, который создает дополнительный слой абстракции для работы с запросами к серверу.
    // Каждый ViewModel, которому будет необходим доступ к серверу, должен создать экземпляр этого класса
    public class ServerContext
    {
        const string url = "https://localhost:7078";
        public ServerContext() {}

        // Метод для создания запросов к серверу
        // Должен иметь реализацию для всех случаев
        //
        // Для получения моделей из базы данных
        // adress: ../Controller/
        public async void GetRequest<T>(string controller, GetModel<T> getModel)
        {
            string apiUrl = $"{url}/api/{controller}";
            try
            {
                await SendGetRequest(apiUrl, getModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        public async void GetRequest<T>(string controller, string id, GetModel<T> getModel)
        {
            string apiUrl = $"{url}/api/{controller}/{id}";
            try
            {
                await SendGetRequest(apiUrl, getModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        // Для получения коллекций из базы данных
        // adress: ../Controller/id
        public async void GetRequest<T>(string controller, GetCollectinModel<T> getCollectinModel)
        {
            string apiUrl = $"{url}/api/{controller}";
            try
            {
                await SendGetRequest(apiUrl, getCollectinModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        public async void GetRequest<T>(string controller, string id, GetCollectinModel<T> getCollectinModel)
        {
            string apiUrl = $"{url}/api/{controller}/{id}";
            try
            {
                await SendGetRequest(apiUrl, getCollectinModel);
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        // Делегат, который передает значения модели со стороны сервера из данного класса в тот класс, который его реализует
        // Позволяет реализующим классам абсрагироваться от http запросов
        public delegate void GetModel<T>(T model);
        public delegate void GetCollectinModel<T>(ICollection<T> model);

        // Метод для получения результов от сервера по запросу
        // Имеет две реализации для моделей и для коллекций
        //
        // Должен принимать делегат для записи значений из серверных моделей в модели на стороне клиента
        private async Task SendGetRequest<T>(string apiUrl, GetModel<T> getModel)
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
        private async Task SendGetRequest<T>(string apiUrl, GetCollectinModel<T> getCollectionModel)
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



        // Метод для создания запросов к серверу
        // Для отправки данных
        // adress: ../Controller/
        public async void PostRequest<P>(string controller, P getModel)
        {
            string apiUrl = $"{url}/api/{controller}";
            try
            {
                await SendPostRequest(apiUrl, getModel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Метод для отправки данных на сервер
        public async Task SendPostRequest<P>(string apiUrl, P getModel)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    // Преобразуем данные в JSON
                    var jsonData = JsonConvert.SerializeObject(getModel);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Отправляем запрос методом POST
                    var response = await httpClient.PostAsync(apiUrl, content);

                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Сообщение успешно отправлено на сервер.");
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при отправке сообщения на сервер. Код ошибки: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при отправке сообщения на сервер: " + ex.Message);
                }
            }
        }
    }
}
