namespace SmsProNikita.Utils
{
    public interface IResponseParser
    {
        T Parse<T>(string xml);
    }
}