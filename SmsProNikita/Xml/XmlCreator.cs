using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using SmsProNikita.Config;

namespace SmsProNikita.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlCreator
    {
        private readonly SmsProNikitaConfig _config;

        /// <summary>
        /// Генерирует XML запроса отправки смс
        /// </summary>
        /// <param name="id">id запроса</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="time">Время отправки</param>
        /// <param name="phones">Телефоные номера</param>
        /// <returns></returns>
        public string CreateSendSmsXml(string id, string text, DateTimeOffset time, IEnumerable<string> phones)
        {
            var xdocument = new XDocument();
            var messageElement = new XElement("message");
            var loginElement = new XElement("login", _config.Login);
            var pwdElement = new XElement("pwd", _config.Password);
            var idElement = new XElement("id", id);
            var senderElement = new XElement("sender", _config.Sender);
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
        public string CreateDeliveryReportXml(string id, string phone = null)
        {
            var xdocument = new XDocument();
            var drElement = new XElement("dr");
            var loginElement = new XElement("login", _config.Login);
            var pwdElement = new XElement("pwd", _config.Password);
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
        public string CreateAccountInfoXml()
        {
            var xdocument = new XDocument();
            var infoElement = new XElement("info");
            var loginElement = new XElement("login", _config.Login);
            var pwdElement = new XElement("pwd", _config.Password);

            infoElement.Add(loginElement);
            infoElement.Add(pwdElement);

            xdocument.Add(infoElement);

            return xdocument.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlCreator"/> class.
        /// </summary>
        /// <param name="smsProNikitaConfig">The SMS pro nikita configuration.</param>
        /// <exception cref="ArgumentNullException">smsProNikitaConfig</exception>
        public XmlCreator(SmsProNikitaConfig smsProNikitaConfig)
        {
            _config = smsProNikitaConfig ?? throw new ArgumentNullException("smsProNikitaConfig");
        }

    }
}
