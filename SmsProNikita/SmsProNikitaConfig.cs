using System;

namespace SmsProNikita
{
    public class SmsProNikitaConfig
    {
        private string _login;
        private string _password;
        private string _sender;

        public string Login
        {
            get => _login;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("argument is null or empty", "value");
                _login = value;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("argument is null or empty", "value");
                _password = value;
            }
        }

        public string Sender
        {
            get => _sender;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("argument is null or empty", "value");
                _sender = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login">Логин выдаваемый при создании аккаунта </param>
        /// <param name="password">Пароль </param>
        /// <param name="sender">Имя отправителя, отображаемое в телефоне получателя. Может состоять либо из 11 латинских букв, цифр и знаков точка и тире, либо из 14 цифр.</param>
        public SmsProNikitaConfig(string login, string password, string sender)
        {
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentException("argument is null or empty", "login");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("argument is null or empty", "password");
            if (string.IsNullOrWhiteSpace(sender)) throw new ArgumentException("argument is null or empty", "sender");

            _login = login;
            _password = password;
            _sender = sender;
        }
    }
}
