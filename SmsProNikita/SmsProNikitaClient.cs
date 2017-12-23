using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SmsProNikita.Types;
using SmsProNikita.Statuses;

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

        private readonly string _login;
        private readonly string _password;
        private readonly string _sender;

        /// <summary>
        /// Инициализирует новый экземпляр класса используя экземпляр класса настроек SmsProNikitaConfig
        /// </summary>
        /// <param name="smsProNikitaConfig">Экземпляр класса настроек</param>
        public SmsProNikitaClient(SmsProNikitaConfig smsProNikitaConfig)
        {
            if (smsProNikitaConfig == null) throw new ArgumentException("argument is null", "smsProNikitaConfig");
            _login = smsProNikitaConfig.Login;
            _password = smsProNikitaConfig.Password;
            _sender = smsProNikitaConfig.Sender;
        }

        /// <summary>
        ///  Инициализирует новый экземпляр класса используя login, password , sender
        /// </summary>
        /// <param name="login">Логин выдаваемый при создании аккаунта</param>
        /// <param name="password">Пароль </param>
        /// <param name="sender">Имя отправителя, отображаемое в телефоне получателя. Может состоять либо из 11 латинских букв, цифр и знаков точка и тире, либо из 14 цифр.</param>
        public SmsProNikitaClient(string login, string password, string sender)
        {
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentException("argument is null or empty", "login");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("argument is null or empty", "password");
            if (string.IsNullOrWhiteSpace(sender)) throw new ArgumentException("argument is null or empty", "sender");

            _login = login;
            _password = password;
            _sender = sender;
        }

        #region ApiMethods

        /// <summary>
        /// Отправляет смс
        /// </summary>
        /// <param name="phone">Номер телефона на который необходимо отправить сообщение. Формат телефона 996555123456 либо +996555123456.</param>
        /// <param name="text">Текст сообщения. Максимальная длина 800 символов. Сообщение при необходимости будет разбито на несколько SMS, каждое из которых тарифицируется отдельно. Размер одного SMS – 160 символов в латинице или 70 символов в кириллице. При разбивке сообщения на части в каждую часть добавляется заголовок для объединения частей в одно сообщение на телефоне получателя, поэтому в длинных сообщениях максимальная длина одной части становится 67 символов для кириллицы и 153 для латинских символов. Формат UTF-8</param>
        /// <param name="id">Id сообщения – любой набор латинских букв и цифр длиной до 12 знаков. id каждой отправки должен быть уникальным. Если у двух отправок указан одинаковый id, то вторая по очереди отправка будет заблокирована. Этот параметр необязателен – если отсутствует, то сгенерируется автоматически</param>
        /// <param name="dateTimeUTC">Время отправки. Этот параметр необязателен – если отсутствует, то сообщение отправляется немедленно</param>
        /// <returns></returns>
        public SendSmsResponse SendSms(string phone, string text, string id = null, DateTime dateTimeUTC = default(DateTime))
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("argument is null or empty", "phone");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("argument is null or empty", "text");
            if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString("N").Substring(0, 11);

            var xml = CreateSendSmsXml(id, text, dateTimeUTC, phone);

            return ParseSendSmsResponse(Request(SendSmsApiUrl, xml));
        }

        /// <summary>
        /// Отправляет смс группе номеров
        /// </summary>
        /// <param name="phones">Номера телефонов на который необходимо отправить сообщение. Формат телефона 996555123456 либо +996555123456. Максимальное число получателей в одном пакете – 50 номеров.</param>
        /// <param name="text">Текст сообщения. Максимальная длина 800 символов. Сообщение при необходимости будет разбито на несколько SMS, каждое из которых тарифицируется отдельно. Размер одного SMS – 160 символов в латинице или 70 символов в кириллице. При разбивке сообщения на части в каждую часть добавляется заголовок для объединения частей в одно сообщение на телефоне получателя, поэтому в длинных сообщениях максимальная длина одной части становится 67 символов для кириллицы и 153 для латинских символов. Формат UTF-8</param>
        /// <param name="id">Id сообщения – любой набор латинских букв и цифр длиной до 12 знаков. id каждой отправки должен быть уникальным. Если у двух отправок указан одинаковый id, то вторая по очереди отправка будет заблокирована. Этот параметр необязателен – если отсутствует, то сгенерируется автоматически</param>
        /// <param name="dateTimeUTC">Время отправки. Этот параметр необязателен – если отсутствует, то сообщение отправляется немедленно</param>
        /// <returns></returns>
        public SendSmsResponse SendSms(string[] phones, string text, string id = null, DateTime dateTimeUTC = default(DateTime))
        {
            if (phones == null || phones.Length == 0 || phones.Length > 50) throw new ArgumentException("count phones must be greate than 0 and less tnan 51", "phones");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("argument is null or empty", "text");

            if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString("N").Substring(0, 11);

            var xml = CreateSendSmsXml(id, text, dateTimeUTC, phones);

            return ParseSendSmsResponse(Request(SendSmsApiUrl, xml));
        }

        /// <summary>
        /// Запрос отчета по отправленным смс
        /// </summary>
        /// <param name="id">id сообщения в котором производилась отправка на определенный номер телефона</param>
        /// <param name="phone">Номер телефона. Необязательное поле – если не указано, то возвращается отчет о всех телефонах транзакции</param>
        /// <returns>Возвращает отчет </returns>
        public DeliveryReport GetDeliveryReport(string id, string phone = null)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("argument is null or empty", "id");
            var xml = CreateDeliveryReportXml(id, phone);
            return ParseDeliveryReportResponse(Request(DeliveryReportApiUrl, xml));
        }

        /// <summary>
        /// Запрос информации о состоянии счета и аккаунта
        /// </summary>
        /// <returns></returns>
        public AccountInfo GetAccountInfo()
        {
            var xml = CreateAccountInfoXml();
            return ParseAccountInfoResponse(Request(AccountInfoApiUrl, xml));
        }

        #endregion

        #region ResponseParsers

        /// <summary>
        /// Парсит ответ запроса отправки смс и мапит к классу SendSmsResponse
        /// </summary>
        /// <param name="responseXml">XML ответ запроса отправки смс</param>
        /// <returns></returns>
        public SendSmsResponse ParseSendSmsResponse(string responseXml)
        {
            try
            {
                var document = new XmlDocument();
                document.LoadXml(responseXml);
                var response = new SendSmsResponse()
                {
                    Id = document.GetElementsByTagName("id").Item(0).InnerText,
                    Status = int.Parse(document.GetElementsByTagName("status").Item(0).InnerText),
                    StatusDescription = SmsProNikitaStatuses.GetSendSmsStatusDescription(int.Parse(document.GetElementsByTagName("status").Item(0).InnerText)),
                    Phones = int.Parse(document.GetElementsByTagName("phones").Item(0).InnerText),
                    SmsPart = int.Parse(document.GetElementsByTagName("smscnt").Item(0).InnerText),
                    Message = document.GetElementsByTagName("message").Item(0)?.InnerText
                };

                return response;
            }
            catch (Exception e)
            {
                throw new Exception("Error Parse Send Sms Response: " + e.Message);
            }
        }

        /// <summary>
        /// Парсит ответ запроса отчета и мапит к классу DeliveryReport
        /// </summary>
        /// <param name="deliveryReportXml">XML ответ запроса отчета</param>
        /// <returns></returns>
        public DeliveryReport ParseDeliveryReportResponse(string deliveryReportXml)
        {
            try
            {
                var document = new XmlDocument();
                document.LoadXml(deliveryReportXml);
                var response = new DeliveryReport
                {
                    Status = int.Parse(document.GetElementsByTagName("status").Item(0).InnerText),
                    StatusDescription = SmsProNikitaStatuses.GetDeliveryReportStatusDescription(int.Parse(document.GetElementsByTagName("status").Item(0).InnerText))
                };

                var phoneList = new List<ReportPhone>();

                var xRoot = document.DocumentElement;
                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name != "phone") continue;
                    var phone = new ReportPhone();
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        switch (childnode.Name)
                        {
                            case "number":
                                phone.Number = childnode.InnerText;
                                break;
                            case "report":
                                phone.Report = int.Parse(childnode.InnerText);
                                phone.ReportDescription = SmsProNikitaStatuses.GetReportStatusesDescription(int.Parse(childnode.InnerText));
                                break;
                            case "sendTime":
                                phone.SendTime = DateTime.ParseExact(childnode.InnerText, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                                break;
                            case "rcvTime":
                                phone.RcvTime = !string.IsNullOrWhiteSpace(childnode.InnerText) ? DateTime.ParseExact(childnode.InnerText, "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : default(DateTime);
                                break;
                        }
                    }
                    phoneList.Add(phone);
                }
                response.Phones = phoneList.ToArray();
                return response;
            }
            catch (Exception e)
            {
                throw new Exception("Error Parse Delivery Report Response: " + e.Message);
            }
        }

        /// <summary>
        /// Парсит ответ запроса информации о состоянии счета и аккаунта
        /// </summary>
        /// <param name="accountInfoXml">XML ответ запроса информации о состоянии счета и аккаунта</param>
        /// <returns></returns>
        public AccountInfo ParseAccountInfoResponse(string accountInfoXml)
        {
            try
            {
                var document = new XmlDocument();
                document.LoadXml(accountInfoXml);
                var response = new AccountInfo()
                {
                    Status = int.Parse(document.GetElementsByTagName("status").Item(0).InnerText),
                    StatusDescription = SmsProNikitaStatuses.GetAccountStatusesDescription(int.Parse(document.GetElementsByTagName("status").Item(0).InnerText)),
                    IsActive = int.Parse(document.GetElementsByTagName("state").Item(0).InnerText) == 0,
                    Account = Double.Parse(document.GetElementsByTagName("account").Item(0).InnerText),
                    SmsPrice = Double.Parse(document.GetElementsByTagName("smsprice").Item(0).InnerText)
                };

                return response;
            }
            catch (Exception e)
            {
                throw new Exception("Error Parse Account Info Response: " + e.Message);
            }
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Отправляет запрос 
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="xml">xml передаваемый в запросе</param>
        /// <returns></returns>
        private string Request(string url, string xml)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var requestBytes = Encoding.UTF8.GetBytes(xml);
            request.Method = "POST";
            request.ContentType = "text/xml;";
            request.ContentLength = requestBytes.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            try
            {
                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Ошибка запроса к API. URL:{0}, XML:{1}, ExceptionDescription:{2}", url, xml, exception.Message));
            }
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
        private string CreateSendSmsXml(string id, string text, DateTime time, params string[] phones)
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

            if (time != default(DateTime))
                messageElement.Add(new XElement("time", time.AddHours(6).ToString("yyyyMMddHHmmss")));

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
