using System;
using System.Net.Http;
using System.Text;
using SmsProNikita.Exceptions;

namespace SmsProNikita.Services
{
    public class HttpService : IHttpService, IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="HttpService"/> класса.
        /// </summary>
        public HttpService()
        {
            _httpClient = new HttpClient();
        }


        /// <summary>
        /// Отправляет запрос на URL с содержищимся в нем XML.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="xml">XML.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// url не может быть пустым - url
        /// или
        /// xml не может быть пустым - xml
        /// </exception>
        /// <exception cref="SmsProNikita.Exceptions.HttpException">Ошибка отправки запроса в HttpService</exception>
        public string Request(string url, string xml)
        {
            if(string.IsNullOrWhiteSpace(url)) throw new ArgumentException("url не может быть пустым","url");
            if(string.IsNullOrWhiteSpace(xml)) throw new ArgumentException("xml не может быть пустым","xml");

            try
            {
                var response = _httpClient.PostAsync(url, new StringContent(xml, Encoding.UTF8, "text/xml")).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                throw new HttpException("Ошибка отправки запроса в HttpService", e);
            }

        }

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
