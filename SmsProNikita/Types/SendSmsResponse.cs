namespace SmsProNikita.Types
{
    /// <summary>
    /// Ответ сервиса smspro.nikita.kg на запрос отправки смс
    /// </summary>
    public class SendSmsResponse
    {
        /// <summary>
        /// Id запроса, который был передан партнером в данном запросе
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Статус отправки
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Описание статуса отправки
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// Число номеров телефонов, распознанное запросе
        /// </summary>
        public int Phones { get; set; }
        /// <summary>
        /// Число частей SMS, на которое разделилось отправляемое сообщение.
        /// </summary>
        public int SmsPart { get; set; }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Message { get; set; }
    }
}
