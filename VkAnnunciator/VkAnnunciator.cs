using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Linq;
using System;
using VkAnnunciator.Log;
using System.Threading;

namespace EgeAnnunciator
{
    public class VkAnnunciator
    {
        private VkApi vkApi = new VkApi();

        private VkSettings vk;
        private AnnunciatorSettings settings;

        private DateTime lastSend = new DateTime(2000, 1, 1);
        private DateTime lastEgeSend = new DateTime(2000, 1, 1);

        private int times = 0;
        private Word days = new Word("день", "дня", "дней");
        private Word hours = new Word("час", "часа", "часов");
        private Word left = new Word("остался", "осталось", "осталось");

        private ILogger Logger;

        public VkAnnunciator(AnnunciatorSettings settings, VkSettings vk, ILogger logger)
        {
            this.settings = settings;
            this.vk = vk;
            Logger = logger;

            ApiAuthParams param = new ApiAuthParams {
                ApplicationId = this.vk.AppId,
                Login = this.vk.Login,
                Password = this.vk.Password,
                Settings = Settings.All
            };

            vkApi.Authorize(param);
            Log($"Initialize annunciator: {this.settings.Id}");
        }

        public void Start()
        {
            while (true) {
                try {
                    if (settings.BeginHour <= DateTime.Now.Hour && DateTime.Now.Hour <= settings.EndHour) {
                        Check();
                    }
                    Thread.Sleep(settings.Interval);
                }
                catch (Exception ex) {
                    Log(ex.Message);
                }
            }
        }

        public void Check()
        {
            // Если с последней отправки прошло меньше MessagesInterval минут, то ничего не делаем
            if ((DateTime.Now - lastSend).TotalMinutes < settings.MessagesInterval) {
                Log($"After the last sending has gone {Convert.ToInt32((DateTime.Now - lastSend).TotalMinutes)} minutes.");
                return;
            }

            // Находим пользователя
            var user = vkApi.Users.Get(new long[] { settings.Id }, ProfileFields.Online | ProfileFields.OnlineMobile).FirstOrDefault();

            // Если пользователь онлайн, то отправляем ему сообщение
            if ((user.Online.HasValue && user.Online.Value) || (user.OnlineMobile.HasValue && user.OnlineMobile.Value)) {

                string message = string.Empty;
                bool isEgeSend = false;

                // Если с последней отправки сообщения о ЕГЭ прошло больше EgeMessagesInterval часов, 
                // то устанавливаем сообщение и флаг
                if ((DateTime.Now - lastEgeSend).TotalHours > settings.EgeMessagesInterval) {

                    foreach (var subject in settings.Subjects) {
                        TimeSpan timeSpan = subject.Date - DateTime.Now;
                        message += GetString(subject.GenitiveName, timeSpan) + Environment.NewLine;
                    }

                    isEgeSend = true;
                }
                else {
                    message = GetMessage(times++);
                }

                // Отправка сообщения
                vkApi.Messages.Send(new MessagesSendParams {
                    Message = message,
                    UserId = user.Id
                });

                if (isEgeSend) {
                    lastEgeSend = DateTime.Now;
                }
                else {
                    lastSend = DateTime.Now;
                }

                // Логируем
                Log($"Message sent to {user.FirstName} {user.LastName}");
                Log(message);
            }
            else {
                Log("offline");
                times = 0;
            }
        }

        private void Log(string logMessage)
        {
            Logger?.Log(logMessage);
        }

        private string GetMessage(int times)
        {
            times = times < settings.Phrases.Length - 1 ? times : settings.Phrases.Length - 1;

            return settings.Phrases[times];
        }

        private string GetString(string name, TimeSpan time)
        {
            return $"До {name.ToUpper()} {left.GetWord(time.Days)} {time.Days} {days.GetWord(time.Days)}!";
        }
    }
}
