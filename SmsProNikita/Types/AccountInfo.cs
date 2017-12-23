namespace SmsProNikita.Types
{
    /// <summary>
    /// Ответ сервиса smspro.nikita.kg на запрос информации о состоянии счета и аккаунта
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// Статус запроса
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Описание статуса запроса
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// Флаг активности аккаунта
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Состояние счета  (положительная сумма денег доступных для расходования без учета возможного кредита - в валюте счета)
        /// </summary>
        public double Account { get; set; }
        /// <summary>
        /// Стоимость одного SMS-сообщения в валюте счета. 
        /// </summary>
        public double SmsPrice { get; set; }
    }
}
