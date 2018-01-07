using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SmsProNikita.Config;
using SmsProNikita.Services;
using SmsProNikita.Types;
using SmsProNikita.Utils;
using SmsProNikita.Xml;

namespace SmsProNikita
{
    /// <summary>
    /// Класс клиент для работы с сервисом рассылки смс smspro.nikita.kg
    /// </summary>
    public class SmsProNikitaClient
    {
        private const string SendSmsApiUrl = @"http://smspro.nikita.kg/api/message";
        private const string DeliveryReportApiUrl = @"http://smspro.nikita.kg/api/dr";
        private const string AccountInfoApiUrl = @"http://smspro.nikita.kg/api/info ";

        private readonly IHttpService _httpService;
        private readonly SmsProNikitaConfig _config;
        private readonly XmlCreator _xmlCreator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SmsProNikitaClient"/> класса.
        /// </summary>
        /// <param name="smsProNikitaConfig">SmsProNikitaConfig</param>
        /// <param name="webProxy">webProxy</param>
        /// <exception cref="ArgumentNullException">smsProNikitaConfig</exception>
        public SmsProNikitaClient(SmsProNikitaConfig smsProNikitaConfig, IWebProxy webProxy = null)
        {
            _config = smsProNikitaConfig ?? throw new ArgumentNullException("smsProNikitaConfig");
            _xmlCreator = new XmlCreator(_config);
            _httpService = webProxy != null ? new HttpService(webProxy) : new HttpService();
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SmsProNikitaClient"/> класса.
        /// </summary>
        /// <param name="login">Логин выдаваемый при создании аккаунта</param>
        /// <param name="password">Пароль</param>
        /// <param name="sender">Имя отправителя, отображаемое в телефоне получателя. Может состоять либо из 11 латинских букв, цифр и знаков точка и тире, либо из 14 цифр.</param>
        /// <param name="webProxy">webProxy</param>
        /// <exception cref="ArgumentException">
        /// Логин не может быть пустым - login
        /// или
        /// Пароль не может быть пустым - password
        /// или
        /// Имя отправителя не может быть пустым - sender
        /// </exception>
        public SmsProNikitaClient(string login, string password, string sender, IWebProxy webProxy = null)
        {
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentException("Логин не может быть пустым", "login");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Пароль не может быть пустым", "password");
            if (string.IsNullOrWhiteSpace(sender)) throw new ArgumentException("Имя отправителя не может быть пустым", "sender");

            _config = new SmsProNikitaConfig(login, password, sender);
            _xmlCreator = new XmlCreator(_config);
            _httpService = webProxy != null ? new HttpService(webProxy) : new HttpService();
        }

        #region ApiMethods

        /// <summary>
        /// Отправляет смс на указанный номер.
        /// </summary>
        /// <param name="phone">Номер телефона на который необходимо отправить сообщение. Формат телефона 996555123456 либо +996555123456.</param>
        /// <param name="text">Текст сообщения. Максимальная длина 800 символов. Сообщение при необходимости будет разбито на несколько SMS, каждое из которых тарифицируется отдельно. Размер одного SMS – 160 символов в латинице или 70 символов в кириллице. При разбивке сообщения на части в каждую часть добавляется заголовок для объединения частей в одно сообщение на телефоне получателя, поэтому в длинных сообщениях максимальная длина одной части становится 67 символов для кириллицы и 153 для латинских символов. Формат UTF-8.</param>
        /// <param name="id">Id сообщения – любой набор латинских букв и цифр длиной до 12 знаков. id каждой отправки должен быть уникальным. Если у двух отправок указан одинаковый id, то вторая по очереди отправка будет заблокирована. Этот параметр необязателен – если отсутствует, то сгенерируется автоматически.</param>
        /// <param name="dateTimeUTC">Время отправки. Этот параметр необязателен – если отсутствует, то сообщение отправляется немедленно. Время бишкекское (GMT+6).</param>
        /// <returns>Возвращает ответ сервиса приведенный к классу SendSmsResponse</returns>
        /// <exception cref="ArgumentException">
        /// телефон не может быть пустым - phone
        /// или
        /// текст сообщения не может быть пустым - text
        /// </exception>
        public SendSmsResponse SendSms(string phone, string text, string id = null, DateTimeOffset dateTimeUTC = default(DateTimeOffset))
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("телефон не может быть пустым", "phone");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("текст сообщения не может быть пустым", "text");

            if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString("N").Substring(0, 11);

            var xml = _xmlCreator.CreateSendSmsXml(id, text, dateTimeUTC, new[] { phone });
            var response = _httpService.Request(SendSmsApiUrl, xml);
            return ResponseParser.ParseSendSmsResponse(response);
        }

        /// <summary>
        /// Отправляет смс группе номеров.
        /// </summary>
        /// <param name="phones">Номера телефонов на который необходимо отправить сообщение. Формат телефона 996555123456 либо +996555123456. Максимальное число получателей в одном пакете – 50 номеров.</param>
        /// <param name="text">Текст сообщения. Максимальная длина 800 символов. Сообщение при необходимости будет разбито на несколько SMS, каждое из которых тарифицируется отдельно. Размер одного SMS – 160 символов в латинице или 70 символов в кириллице. При разбивке сообщения на части в каждую часть добавляется заголовок для объединения частей в одно сообщение на телефоне получателя, поэтому в длинных сообщениях максимальная длина одной части становится 67 символов для кириллицы и 153 для латинских символов. Формат UTF-8.</param>
        /// <param name="id">Id сообщения – любой набор латинских букв и цифр длиной до 12 знаков. id каждой отправки должен быть уникальным. Если у двух отправок указан одинаковый id, то вторая по очереди отправка будет заблокирована. Этот параметр необязателен – если отсутствует, то сгенерируется автоматически.</param>
        /// <param name="dateTimeUTC">Время отправки. Этот параметр необязателен – если отсутствует, то сообщение отправляется немедленно. Время бишкекское (GMT+6).</param>
        /// <returns>Возвращает ответ сервиса приведенный к классу SendSmsResponse</returns>
        /// <exception cref="ArgumentException">
        /// количество номеров в запросе должно быть больше 0 и не должно превышать 50 - phones
        /// или
        /// текст сообщения не может быть пустым - text
        /// </exception>
        public SendSmsResponse SendSms(IEnumerable<string> phones, string text, string id = null, DateTimeOffset dateTimeUTC = default(DateTimeOffset))
        {
            if (phones == null || phones.Count() == 0 || phones.Count() > 50) throw new ArgumentException("количество номеров в запросе не должнобыть или превышать 50", "phones");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("текст сообщения не может быть пустым", "text");

            if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString("N").Substring(0, 11);

            var xml = _xmlCreator.CreateSendSmsXml(id, text, dateTimeUTC, phones);
            var response = _httpService.Request(SendSmsApiUrl, xml);
            return ResponseParser.ParseSendSmsResponse(response);
        }

        /// <summary>
        /// Запрос отчета по отправленным смс
        /// </summary>
        /// <param name="id">id сообщения в котором производилась отправка на определенный номер телефона</param>
        /// <param name="phone">Номер телефона. Необязательное поле – если не указано, то возвращается отчет о всех телефонах транзакции</param>
        /// <returns>Возвращает отчет преденный к классу DeliveryReport</returns>
        /// /// <exception cref="ArgumentException">
        /// id запроса не может быть пустым - id
        /// </exception>
        public DeliveryReport GetDeliveryReport(string id, string phone = null)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id запроса не может быть пустым", "id");

            var xml = _xmlCreator.CreateDeliveryReportXml(id, phone);
            var response = _httpService.Request(DeliveryReportApiUrl, xml);
            return ResponseParser.ParseDeliveryReportResponse(response);
        }

        /// <summary>
        /// Запрос информации о состоянии счета и аккаунта
        /// </summary>
        /// <returns> Возвращает информацию о состоянии счета и аккаунта приведенную к классу AccountInfo</returns>
        public AccountInfo GetAccountInfo()
        {
            var xml = _xmlCreator.CreateAccountInfoXml();
            var response = _httpService.Request(AccountInfoApiUrl, xml);
            return ResponseParser.ParseAccountInfoResponse(response);
        }

        #endregion

    }
}
