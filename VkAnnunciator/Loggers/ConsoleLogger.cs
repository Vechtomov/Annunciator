namespace VkAnnunciator.Loggers
{
    /// <summary>
    /// Консольный логгер
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Log(string logMessage)
        {
            System.Console.WriteLine(logMessage);
        }
    }
}
