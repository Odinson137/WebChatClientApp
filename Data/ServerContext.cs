using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Text;
using System.Linq;
using System.Net;

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
        public async void GetRequest<G>(string controller, GetModel<G> getModel)
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

        public async void GetRequest<G>(string controller, object id, GetModel<G> getModel)
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

        // Делегат, который передает значения модели со стороны сервера из данного класса в тот класс, который его реализует
        // Позволяет реализующим классам абсрагироваться от http запросов
        public delegate void GetModel<T>(T model);
 
        // Метод для получения результов от сервера по запросу
        // Имеет две реализации для моделей и для коллекций
        //
        // Должен принимать делегат для записи значений из серверных моделей в модели на стороне клиента
        private static async Task SendGetRequest<G>(string apiUrl, GetModel<G> getModel)
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
                    G result = JsonConvert.DeserializeObject<G>(responseBody);
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

        public async void PostRequestUrl(string controller, Dictionary<string, object> parameters)
        {
            string apiUrl = $"{url}/api/{controller}";
            try
            {
                var query = string.Join("&", parameters
                    .Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value.ToString())}"));

                var fullUrl = $"{apiUrl}?{query}";
                await SendPostRequestUrl(fullUrl);
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
                    var jsonData = JsonConvert.SerializeObject(getModel);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        //MessageBox.Show("Сообщение успешно отправлено на сервер.");
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

        // Метод для отправки данных на сервер с уже готовым url с параметрами
        public async Task SendPostRequestUrl(string fullUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var content = new StringContent("", Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(fullUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        //MessageBox.Show("Сообщение успешно отправлено на сервер.");
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при отправке запроса на сервер. Код ошибки: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при отправке запроса на сервер: " + ex.Message);
                }
            }
        }

        // Метод для создания запросов к серверу
        // Для изменения данных в моделе
        // adress: ../Controller/
        public async void PutRequestUrl(string controller, Dictionary<string, object> parameters)
        {
            string apiUrl = $"{url}/api/{controller}";
            try
            {
                var query = string.Join("&", parameters
                    .Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value.ToString())}"));

                var fullUrl = $"{apiUrl}?{query}";
                await SendPutRequestUrl(fullUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Метод для изменения данных на сервере по запросу
        private static async Task SendPutRequestUrl(string fullUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var content = new StringContent("", Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync(fullUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Произошла ошибка при отправке сообщения на сервер. Код ошибки: " + response.StatusCode);
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Ошибка при отправке запроса: " + ex.Message);
                    throw new Exception("Ошибка при отправке запроса: " + ex.Message);
                }
            }
        }


        // Метод для создания запросов к серверу
        // Для удаления данных из базы
        // adress: ../Controller/id
        public async void PutRequestUrl(string controller, object id)
        {
            string apiUrl = $"{url}/api/{controller}/{id}";
            try
            {
                await SendDeleteRequestUrl(apiUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Метод для удаления данных на сервере по запросу
        private static async Task SendDeleteRequestUrl(string fullUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.DeleteAsync(fullUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Произошла ошибка при отправке сообщения на сервер. Код ошибки: " + response.StatusCode);
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Ошибка при отправке запроса: " + ex.Message);
                    throw new Exception("Ошибка при отправке запроса: " + ex.Message);
                }
            }
        }

    }
}
