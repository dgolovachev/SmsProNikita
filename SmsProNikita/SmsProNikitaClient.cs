using System;
using System.Xml.Linq;
using SmsProNikita.Services;
using SmsProNikita.Types;
using SmsProNikita.Utils;

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
        private readonly string _login;
        private readonly string _password;
        private readonly string _sender;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SmsProNikitaClient"/> класса.
        /// </summary>
        /// <param name="smsProNikitaConfig">SmsProNikitaConfig</param>
        /// <exception cref="ArgumentNullException">smsProNikitaConfig</exception>
        public SmsProNikitaClient(SmsProNikitaConfig smsProNikitaConfig)
        {
            if (smsProNikitaConfig == null) throw new ArgumentNullException("smsProNikitaConfig");

            _login = smsProNikitaConfig.Login;
            _password = smsProNikitaConfig.Password;
            _sender = smsProNikitaConfig.Sender;

            _httpService = new HttpService();
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SmsProNikitaClient"/> класса.
        /// </summary>
        /// <param name="login">Логин выдаваемый при создании аккаунта</param>
        /// <param name="password">Пароль</param>
        /// <param name="sender">Имя отправителя, отображаемое в телефоне получателя. Может состоять либо из 11 латинских букв, цифр и знаков точка и тире, либо из 14 цифр.</param>
        /// <exception cref="ArgumentException">
        /// Логин не может быть пустым - login
        /// или
        /// Пароль не может быть пустым - password
        /// или
        /// Имя отправителя не может быть пустым - sender
        /// </exception>
        public SmsProNikitaClient(string login, string password, string sender)
        {
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentException("Логин не может быть пустым", "login");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Пароль не может быть пустым", "password");
            if (string.IsNullOrWhiteSpace(sender)) throw new ArgumentException("Имя отправителя не может быть пустым", "sender");

            _login = login;
            _password = password;
            _sender = sender;

            _httpService = new HttpService();
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

            var xml = CreateSendSmsXml(id, text, dateTimeUTC, phone);
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
        public SendSmsResponse SendSms(string[] phones, string text, string id = null, DateTimeOffset dateTimeUTC = default(DateTimeOffset))
        {
            if (phones == null || phones.Length == 0 || phones.Length > 50) throw new ArgumentException("количество номеров в запросе не должнобыть или превышать 50", "phones");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("текст сообщения не может быть пустым", "text");

            if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString("N").Substring(0, 11);

            var xml = CreateSendSmsXml(id, text, dateTimeUTC, phones);
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

            var xml = CreateDeliveryReportXml(id, phone);
            var response = _httpService.Request(DeliveryReportApiUrl, xml);
            return ResponseParser.ParseDeliveryReportResponse(response);
        }

        /// <summary>
        /// Запрос информации о состоянии счета и аккаунта
        /// </summary>
        /// <returns> Возвращает информацию о состоянии счета и аккаунта приведенную к классу AccountInfo</returns>
        public AccountInfo GetAccountInfo()
        {
            var xml = CreateAccountInfoXml();
            var response = _httpService.Request(AccountInfoApiUrl, xml);
            return ResponseParser.ParseAccountInfoResponse(response);
        }

        #endregion


        #region XmlGenerators

        /// <summary>
        /// Генерирует XML запроса отправки смс
        /// </summary>
        /// <param name="id">id запроса</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="time">Время отправки</param>
        /// <param name="phones">Телефоные номера</param>
        /// <returns></returns>
        private string CreateSendSmsXml(string id, string text, DateTimeOffset time, params string[] phones)
        {
            var xdocument = new XDocument();
            var messageElement = new XElement("message");
            var loginElement = new XElement("login", _login);
            var pwdElement = new XElement("pwd", _password);
            var idElement = new XElement("id", id);
            var senderElement = new XElement("sender", _sender);
            var textElement = new XElement("text", text);
            var phonesElement = new XElement("phones");

            messageElement.Add(loginElement);
            messageElement.Add(pwdElement);
            messageElement.Add(idElement);
            messageElement.Add(senderElement);
            messageElement.Add(textElement);

            if (time != default(DateTimeOffset) && time > new DateTimeOffset(DateTime.Now, new TimeSpan(6, 0, 0)))
                messageElement.Add(new XElement("time", time.ToString("yyyyMMddHHmmss")));

            foreach (var phone in phones)
            {
                phonesElement.Add(new XElement("phone", phone));
            }

            messageElement.Add(phonesElement);

            xdocument.Add(messageElement);

            return xdocument.ToString();
        }

        /// <summary>
        /// Генерирует XML запроса отчета
        /// </summary>
        /// <param name="id">id сообщения в котором производилась отправка на определенный номер телефона </param>
        /// <param name="phone">Номер телефона. Необязательное поле – если не указано, то возвращается отчет о всех телефонах транзакции. </param>
        /// <returns></returns>
        private string CreateDeliveryReportXml(string id, string phone = null)
        {
            var xdocument = new XDocument();
            var drElement = new XElement("dr");
            var loginElement = new XElement("login", _login);
            var pwdElement = new XElement("pwd", _password);
            var idElement = new XElement("id", id);

            drElement.Add(loginElement);
            drElement.Add(pwdElement);
            drElement.Add(idElement);

            if (!string.IsNullOrWhiteSpace(phone)) drElement.Add(new XElement("phone", phone));

            xdocument.Add(drElement);

            return xdocument.ToString();
        }

        /// <summary>
        /// Генерирует XML запроса информации о состоянии счета и аккаунта
        /// </summary>
        /// <returns></returns>
        private string CreateAccountInfoXml()
        {
            var xdocument = new XDocument();
            var infoElement = new XElement("info");
            var loginElement = new XElement("login", _login);
            var pwdElement = new XElement("pwd", _password);

            infoElement.Add(loginElement);
            infoElement.Add(pwdElement);

            xdocument.Add(infoElement);

            return xdocument.ToString();
        }

        #endregion

    }
}
