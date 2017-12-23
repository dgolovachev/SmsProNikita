using System;

namespace SmsProNikita.Types
{
    /// <summary>
    /// Элемент запроса отчета о доставке
    /// </summary>
   public class ReportPhone
    {
        /// <summary>
        /// Номер
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Код отчета о доставке.
        /// </summary>
        public int Report { get; set; }
        /// <summary>
        /// Описание кода отчета о доставке
        /// </summary>
        public string ReportDescription { get; set; }
        /// <summary>
        /// Время фактической отправки оператору
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// Время получения отчета о доставке
        /// </summary>
        public DateTime RcvTime { get; set; }
    }
}
