using System;
using System.IO;
using System.Text;
using System.Threading;

namespace VkAnnunciator.Loggers
{
    /// <summary>
    /// Файловый логгер
    /// </summary>
    public class FileLogger : ILogger
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        private string filename;

        /// <summary>
        /// Имя директории
        /// </summary>
        private string directory;

        /// <summary>
        /// Объект для блокировки
        /// </summary>
        private object locker = new object();

        public FileLogger(string filename, string directory)
        {
            this.filename = filename;
            this.directory = directory;

            DirectoryInfo dir = new DirectoryInfo(directory);

            // Если директории нет, то создаем ее
            if (!dir.Exists)
                dir.Create();
        }

        public void Log(string logMessage)
        {
            lock (locker) {
                byte[] buff = Encoding.UTF8.GetBytes($"{DateTime.Now.ToShortTimeString()} : {logMessage}{Environment.NewLine}");
                using (FileStream fileStream = new FileStream($"{directory}\\{filename}.txt", FileMode.Append, FileAccess.Write)) {
                    fileStream.WriteAsync(buff, 0, buff.Length);
                }
            }
        }
    }
}
