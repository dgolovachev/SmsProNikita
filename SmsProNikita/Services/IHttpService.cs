namespace SmsProNikita.Services
{
    public interface IHttpService
    {
        /// <summary>
        /// Отправляет запрос на URL с содержищимся в нем XML.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="xml">XML.</param>
        /// <returns></returns>
        string Request(string url, string xml);
    }
}