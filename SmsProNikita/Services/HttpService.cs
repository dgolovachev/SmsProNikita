using System;
using System.Net;
using System.Net.Http;
using System.Text;
using SmsProNikita.Exceptions;

namespace SmsProNikita.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="SmsProNikita.Services.IHttpService" />
    /// <seealso cref="System.IDisposable" />
    public class HttpService : IHttpService, IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="HttpService"/> класса.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        public HttpService(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="HttpService"/> класса.
        /// </summary>
        /// <param name="webProxy">The web proxy.</param>
        public HttpService(IWebProxy webProxy)
        {
            var httpClientHander = new HttpClientHandler
            {
                Proxy = webProxy,
                UseProxy = true
            };
            _httpClient = new HttpClient(httpClientHander);
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
        /// <exception cref="HttpException">Ошибка отправки запроса в HttpService</exception>
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
