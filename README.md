# SmsProNikita
C# клиент для работы с сервисом рассылки смс smspro.nikita.kg

Пример отправки единичного сообщения:

var config = new SmsProNikitaConfig("LOGIN", "PASSWORD", "SENDERNAME");

var client = new SmsProNikitaClient(config);

var phone = "996707000000";

var text = "test";

client.SendSms(phone, text);

Пример массовой отправки сообщения:

var config = new SmsProNikitaConfig("LOGIN", "PASSWORD", "SENDERNAME");

var client = new SmsProNikitaClient(config);

var phones = new [] { "996707000000", "996707000001" };

var text = "test";

client.SendSms(phones, text);

