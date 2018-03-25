namespace VkAnnunciator.Loggers
{
    /// <summary>
    /// Логгер, который не логгирует ( ._.)
    /// </summary>
    public class NowhereLogger : ILogger
    {
        public void Log(string logMessage) { }
    }
}
