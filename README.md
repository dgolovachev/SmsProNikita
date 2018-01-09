# SmsProNikita
C# клиент для работы с сервисом рассылки смс smspro.nikita.kg

Пример отправки единичного сообщения:
```cs
var config = new SmsProNikitaConfig("LOGIN", "PASSWORD", "SENDERNAME");
var client = new SmsProNikitaClient(config);
var phone = "996707000000";
var text = "test";
var response = client.SendSms(phone, text);
Console.WriteLine($"статус отправки: {response.StatusDescription}");

Thread.Sleep(3000);

// Запрос отчета о доставке
var report = client.GetDeliveryReport(response.Id);

Console.WriteLine($"статус запроса отчета : {report.StatusDescription}");

foreach (var item in report.Phones)
{
    Console.WriteLine($"Номер: {item.Number}, Время отправки: {item.SendTime}, Описание кода отчета о доставке: {item.ReportDescription}, Время получения отчета о доставке: {item.RcvTime}");
}
```

Пример массовой отправки сообщения:
```cs
var config = new SmsProNikitaConfig("LOGIN", "PASSWORD", "SENDERNAME");
var client = new SmsProNikitaClient(config);
var phones = new[] { "996707000000", "996707000001" };
var text = "test";
var response = client.SendSms(phones, text);
Console.WriteLine($"статус отправки: {response.StatusDescription}");

Thread.Sleep(3000);

// Запрос отчета о доставке
var report = client.GetDeliveryReport(response.Id);

Console.WriteLine($"статус запроса отчета : {report.StatusDescription}");

foreach (var item in report.Phones)
{
  Console.WriteLine($"Номер: {item.Number}, Время отправки: {item.SendTime}, Описание кода отчета о доставке: {item.ReportDescription}, Время получения отчета о доставке: {item.RcvTime}");
}
```
