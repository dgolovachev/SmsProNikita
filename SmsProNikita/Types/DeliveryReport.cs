namespace SmsProNikita.Types
{
    /// <summary>
    ///  Ответ сервиса на запрос отчета о доставке 
    /// </summary>
    public class DeliveryReport
    {
        /// <summary>
        /// Код код запроса
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Описание кода запроса
        /// </summary>
        public string StatusDescription { get; set; }
       /// <summary>
       /// Массив элементов отчета
       /// </summary>
        public ReportPhone[] Phones { get; set; }
    }
}
