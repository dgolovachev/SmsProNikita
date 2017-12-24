using System;

namespace SmsProNikita.Exceptions
{
    public class ParseResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResponseException"/> class.
        /// </summary>
        public ParseResponseException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResponseException"/> class.
        /// </summary>
        /// <param name="message">Сообщение, описывающее ошибку.</param>
        public ParseResponseException(String message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResponseException"/> class.
        /// </summary>
        /// <param name="message">Сообщение об ошибке, указывающее причину создания исключения.</param>
        /// <param name="innerException">Исключение, вызвавшее текущее исключение, или пустая ссылка, если внутреннее исключение не задано.</param>
        public ParseResponseException(String message, Exception innerException) : base(message, innerException) { }
    }
}
