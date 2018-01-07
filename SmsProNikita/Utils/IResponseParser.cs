namespace SmsProNikita.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResponseParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        T Parse<T>(string xml);
    }
}