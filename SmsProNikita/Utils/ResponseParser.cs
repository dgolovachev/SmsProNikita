using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using SmsProNikita.Exceptions;
using SmsProNikita.Statuses;
using SmsProNikita.Types;

namespace SmsProNikita.Utils
{
    /// <summary>
    /// Парсит ответы сервиса
    /// </summary>
    public static class ResponseParser
    {
        #region ResponseParsers

        /// <summary>
        /// Парсит ответ запроса отправки смс и мапит к классу SendSmsResponse
        /// </summary>
        /// <param name="xml">XML ответ запроса отправки смс</param>
        /// <returns>Возвращает ответ приведенный к классу SendSmsResponse</returns>
        public static SendSmsResponse ParseSendSmsResponse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentException("xml не может быть пустым", "xml");
            try
            {
                var document = new XmlDocument();
                document.LoadXml(xml);
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
                throw new ParseResponseException("Error Parse Send Sms Response", e);
            }
        }

        /// <summary>
        /// Парсит ответ запроса отчета и мапит к классу DeliveryReport
        /// </summary>
        /// <param name="xml">XML ответ запроса отчета</param>
        /// <returns>Возвращает ответ приведенный к классу DeliveryReport</returns>
        public static DeliveryReport ParseDeliveryReportResponse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentException("xml не может быть пустым", "xml");
            try
            {
                var document = new XmlDocument();
                document.LoadXml(xml);
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
                throw new ParseResponseException("Error Parse Delivery Report Response", e);
            }
        }

        /// <summary>
        /// Парсит ответ запроса информации о состоянии счета и аккаунта
        /// </summary>
        /// <param name="xml">XML ответ запроса информации о состоянии счета и аккаунта</param>
        /// <returns>Возвращает ответ приведенный к классу AccountInfo</returns>
        public static AccountInfo ParseAccountInfoResponse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentException("xml не может быть пустым", "xml");
            try
            {
                var document = new XmlDocument();
                document.LoadXml(xml);
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
                throw new ParseResponseException("Error Parse Account Info Response", e);
            }
        }

        #endregion
    }
}
