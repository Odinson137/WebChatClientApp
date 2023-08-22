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
        static string? token;
        public ServerContext() {}
        //public ServerContext(string _token)
        //{
        //    token = _token;
        //}

        // Метод для создания запросов к серверу
        // Должен иметь реализацию для всех случаев
        //
        // Для получения моделей из базы данных
        // adress: ../Controller/
        public async Task GetRequest<G>(string controller, GetModel<G> getModel)
        {
            string apiUrl = $"{url}/api/{controller}";
            await SendGetRequest(apiUrl, getModel);
        }

        public async Task GetRequest<G>(string controller, object id, GetModel<G> getModel)
        {
            string apiUrl = $"{url}/api/{controller}/{id}";
            await SendGetRequest(apiUrl, getModel);
        }

        // Делегат, который передает значения модели со стороны сервера из данного класса в тот класс, который его реализует
        // Позволяет реализующим классам абсрагироваться от http запросов
        public delegate void GetModel<T>(T model);

        // Делегат, который принимает ответы со стороны сервера об успешности или провале добавления/обновления/удаления данных
        // является дополнительным параметром, так как не всегда важно что либо принимать со стороны сервера после отправки данных
        public delegate void GetValueFromServer(string model);

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
                    //httpClient.BaseAddress = new Uri(url);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    
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
                    Application.Current.Shutdown();
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
        public async Task PostRequest<P>(string controller, P getModel, GetValueFromServer getValue = null, GetValueFromServer failedError = null)
        {
            string apiUrl = $"{url}/api/{controller}";
            await SendPostRequest(apiUrl, getModel, getValue, failedError);
        }

        public async void PostRequestUrl(string controller, GetValueFromServer getValue = null, GetValueFromServer failedError = null)
        {
            string apiUrl = $"{url}/api/{controller}";

            await SendPostRequestUrl(apiUrl, getValue, failedError);
        }

        public async void PostRequestUrl(string controller, Dictionary<string, object> parameters, GetValueFromServer getValue = null, GetValueFromServer failedError = null)
        {
            string apiUrl = $"{url}/api/{controller}";
            var query = string.Join("&", parameters
                .Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value.ToString())}"));

            var fullUrl = $"{apiUrl}?{query}";
            await SendPostRequestUrl(fullUrl, getValue, failedError);
        }

        // Метод для отправки данных на сервер
        public async Task SendPostRequest<P>(string apiUrl, P getModel, GetValueFromServer getValue, GetValueFromServer failedError)
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
                        if (getValue != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            getValue(responseContent);
                        }
                    }
                    else
                    {
                        if (failedError != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            failedError(responseContent);
                        } else 
                        {
                            MessageBox.Show("Произошла ошибка при отправке запроса на сервер. Код ошибки: " + response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (failedError != null)
                    {
                        failedError("No connection");
                    }
                    else
                    {
                        MessageBox.Show("Произошла ошибка при отправке запроса на сервер. Код ошибки: " + ex.Message);
                    }
                }
            }
        }

        // Метод для отправки данных на сервер с уже готовым url с параметрами
        public async Task SendPostRequestUrl(string fullUrl, GetValueFromServer getValue, GetValueFromServer failedError)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    //httpClient.BaseAddress = new Uri(url);
                    //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var content = new StringContent("", Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(fullUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        if (getValue != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            getValue(responseContent);
                        }
                    }
                    else
                    {
                        if (failedError != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            failedError(responseContent);
                        } else
                        {
                            MessageBox.Show("Произошла ошибка при отправке запроса на сервер. Код ошибки: " + response.StatusCode);
                        }
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
            var query = string.Join("&", parameters
                .Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value.ToString())}"));

            var fullUrl = $"{apiUrl}?{query}";
            await SendPutRequestUrl(fullUrl);
        }

        public async void PutRequestUrl(string controller, GetValueFromServer getValue = null, GetValueFromServer failedError = null)
        {
            string apiUrl = $"{url}/api/{controller}";

            await SendPutRequestUrl(apiUrl, getValue, failedError);
        }

        // Метод для изменения данных на сервере по запросу
        private static async Task SendPutRequestUrl(string fullUrl, GetValueFromServer getValue = null, GetValueFromServer failedError = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.DeleteAsync(fullUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        if (getValue != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            getValue(responseContent);
                        }
                    }
                    else
                    {
                        if (failedError != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            failedError(responseContent);
                        }
                        else
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show("Произошла ошибка при отправке запроса на сервер. Код ошибки: " + response.StatusCode);
                        }
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
        public async Task DeleteRequestUrl(string controller, object id)
        {
            string apiUrl = $"{url}/api/{controller}/{id}";
            await SendDeleteRequestUrl(apiUrl);
        }

        public async Task DeleteRequestUrl(string controller, GetValueFromServer getValue = null, GetValueFromServer failedError = null)
        {
            string apiUrl = $"{url}/api/{controller}";

            await SendPutRequestUrl(apiUrl, getValue, failedError);
        }


        // Метод для удаления данных на сервере по запросу
        private static async Task SendDeleteRequestUrl(string fullUrl, GetValueFromServer getValue = null, GetValueFromServer failedError = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.DeleteAsync(fullUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        if (getValue != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            getValue(responseContent);
                        }
                    }
                    else
                    {
                        if (failedError != null)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            failedError(responseContent);
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при отправке запроса на сервер. Код ошибки: " + response.StatusCode);
                        }
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
