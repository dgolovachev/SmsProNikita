using System.Collections.Generic;

namespace SmsProNikita.Statuses
{
    /// <summary>
    /// Класс для работы со статусами
    /// </summary>
    public static class SmsProNikitaStatuses
    {
        /// <summary>
        /// Статусы отправки смс
        /// </summary>
        public static readonly Dictionary<int, string> SendStatuses = new Dictionary<int, string>()
        {
            {0, "Сообщение успешно принято к отправке"},
            {1, "Ошибка в формате запроса"},
            {2, "Неверная авторизация"},
            {3, "Недопустимый IP-адрес отправителя"},
            {4, "Недостаточно средств на счету"},
            {5, "Недопустимое имя отправителя (значение поля sender в запросе не валидировано администратором smspro.nikita.kg)"},
            {6, "Сообщение заблокировано по стоп-словам (в сообщении содержатся слова, блокируемые роботом. Например, нецензурная лексика)"},
            {7, "Некорректный номер"},
            {8, "Неверный формат времени отправки"},
            {9, "Отправка заблокирована из-за срабатывания SPAM фильтра"},
            {10, "Отправка заблокирована из-за последовательного повторения id (ошибочная переотправка)"},
            {11, "Сообщение успешно обработано, но не принято к отправке и не протарифицировано т.к. в запросе был установлен параметр <test>1</test>" }
        };

        /// <summary>
        /// Статусы запроса отчета
        /// </summary>
        public static readonly Dictionary<int, string> DeliveryReportStatuses = new Dictionary<int, string>()
        {
            { 0, "Запрос корректен"},
            { 1, "Ошибка в формате запроса"},
            { 2, "Неверная авторизация"},
            { 3, "Недопустимый IP-адрес отправителя"},
            { 4, "Отчет для указанных номера телефона и ID не найден"}
        };

       /// <summary>
       /// Статусы отправленных смс
       /// </summary>
        public static readonly Dictionary<int, string> ReportStatuses = new Dictionary<int, string>()
        {
             {0, "Сообщение находится в очереди на отправку"                                 },
             {1, "Сообщение отправлено (передано оператору)"                                 },
             {2, "Сообщение отклонено"                                                       },
             {3, "Сообщение успешно доставлено"                                              },
             {4, "Сообщение не доставлено"                                                   },
             {5, "Сообщение не отправлено из-за нехватки средств на счету партнера"          },
             {6, "Неизвестный (новый) статус отправки"                                       }
        };

        /// <summary>
        /// Статусы запроса информации о состоянии счета и аккаунта
        /// </summary>
        public static readonly Dictionary<int, string> AccountStatuses = new Dictionary<int, string>()
        {
            {0, "Запрос корректен"},
            {1, "Ошибка в формате запроса"},
            {2, "Неверная авторизация"},
            {3, "Недопустимый IP-адрес отправителя"}
        };

        /// <summary>
        ///  Возвращает описание статуса запроса информации о состоянии счета и аккаунта
        /// </summary>
        /// <param name="status">Статус</param>
        /// <returns></returns>
        public static string GetAccountStatusesDescription(int status)
        {
            if (AccountStatuses.ContainsKey(status))
                return AccountStatuses[status];
            return "Неизвестный статус " + status;
        }

        /// <summary>
        /// Возвращает описание статуса отчета смс
        /// </summary>
        /// <param name="status">Статус </param>
        /// <returns></returns>
        public static string GetReportStatusesDescription(int status)
        {
            if (ReportStatuses.ContainsKey(status))
                return ReportStatuses[status];
            return "Неизвестный статус " + status;
        }

        /// <summary>
        /// Возвращает описание статуса запроса отчета
        /// </summary>
        /// <param name="status">Статус </param>
        /// <returns></returns>
        public static string GetDeliveryReportStatusDescription(int status)
        {
            if (DeliveryReportStatuses.ContainsKey(status))
                return DeliveryReportStatuses[status];
            return "Неизвестный статус " + status;
        }

        /// <summary>
        /// Возвращает описание статуса отправки смс
        /// </summary>
        /// <param name="status">Статус </param>
        /// <returns></returns>
        public static string GetSendSmsStatusDescription(int status)
        {
            if (SendStatuses.ContainsKey(status))
                return SendStatuses[status];
            return "Неизвестный статус " + status;
        }

    }
}
