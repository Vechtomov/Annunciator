using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Linq;
using System;
using VkAnnunciator.Loggers;
using System.Threading;
using VkAnnunciator.Settings;
using System.Threading.Tasks;

namespace Annunciator
{
    /// <summary>
    /// Сигнализатор - посылает пользователю сообщения с заданным интервалом
    /// </summary>
    public class VkAnnunciator
    {
        /// <summary>
        /// API для работы с вк
        /// </summary>
        private VkApi vkApi = new VkApi();

        /// <summary>
        /// Настройки вк
        /// </summary>
        private VkSettings vk;

        /// <summary>
        /// Настройки сигнализатора
        /// </summary>
        private AnnunciatorSettings settings;

        /// <summary>
        /// Дата последней отправки фразы
        /// </summary>
        private DateTime lastPhrasesSend = new DateTime(2000, 1, 1);

        /// <summary>
        /// Дата последнего уведомительного сообщения
        /// </summary>
        private DateTime lastSubjectSend = new DateTime(2000, 1, 1);

        /// <summary>
        /// Сколько фраз было отправлено подряд
        /// </summary>
        private int times = 0;

        // Вспомогательные объекты для отправки корректного сообщения
        private Word days = null;
        private Word hours = null;
        private Word minutes = null;
        private Word left = new Word("остался", "осталось", "осталось");

        /// <summary>
        /// Логгер
        /// </summary>
        private ILogger logger;

        public VkAnnunciator(AnnunciatorSettings settings, VkSettings vk, ILogger logger)
        {
            this.settings = settings;
            this.vk = vk;
            this.logger = logger;

            // Авторизация вк
            ApiAuthParams param = new ApiAuthParams {
                ApplicationId = this.vk.AppId,
                Login = this.vk.Login,
                Password = this.vk.Password,
                Settings = Settings.All
            };

            vkApi.Authorize(param);


            // Проверка формата времени уведомления
            if (settings.AnnunciationFormat.Contains("d"))
                days = new Word("день", "дня", "дней");

            if (settings.AnnunciationFormat.Contains("h"))
                hours = new Word("час", "часа", "часов");

            if (settings.AnnunciationFormat.Contains("m"))
                minutes = new Word("минута", "минуты", "минут");

            // Логируем
            Log($"Initialize annunciator: {this.settings.Id}");
        }

        /// <summary>
        /// Запуск сигнализатора
        /// </summary>
        public void Start()
        {
            while (true) {
                try {
                    if (settings.BeginHour <= DateTime.Now.Hour && DateTime.Now.Hour <= settings.EndHour) {
                        Check();
                    }
                    Thread.Sleep(settings.RequestInterval);
                }
                catch (Exception ex) {
                    Log(ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод для проверки онлайна пользователя и отправки сообщения
        /// </summary>
        public void Check()
        {
            // Если с последней отправки прошло меньше MessagesInterval минут, то ничего не делаем
            if ((DateTime.Now - lastPhrasesSend) < settings.PhrasesInterval) {
                Log($"After the last sending has gone {Convert.ToInt32((DateTime.Now - lastPhrasesSend).TotalMinutes)} minutes.");
                return;
            }

            // Находим пользователя
            var user = vkApi.Users.Get(new long[] { settings.Id }, ProfileFields.Online | ProfileFields.OnlineMobile).FirstOrDefault();

            // Если пользователь онлайн, то отправляем ему сообщение
            if ((user.Online != null && (bool)user.Online) || (user.OnlineMobile != null && (bool)user.OnlineMobile)) {

                string message = string.Empty;
                bool isSubjectSend = false;

                // Если с последней отправки сообщения о событии прошло больше AnnunciationMessagesInterval, 
                // то устанавливаем сообщение и флаг
                if ((DateTime.Now - lastSubjectSend) > settings.AnnunciationMessagesInterval) {

                    foreach (var subject in settings.Subjects) {
                        TimeSpan timeSpan = subject.Date - DateTime.Now;
                        message += GetAnnunciationMessage(subject.GenitiveName, timeSpan) + Environment.NewLine;
                    }

                    isSubjectSend = true;
                }
                else {
                    message = GetPhrase(times++);
                }

                // Отправка сообщения
                vkApi.Messages.Send(new MessagesSendParams {
                    Message = message,
                    UserId = user.Id
                });

                // Обновляем дату последней отправки
                if (isSubjectSend) {
                    lastSubjectSend = DateTime.Now;
                }
                else {
                    lastPhrasesSend = DateTime.Now;
                }

                // Логгируем
                Log($"Message sent to {user.FirstName} {user.LastName}");
                Log(message);
            }
            else {
                Log("offline");
                times = 0;
            }
        }

        #region Async
        // Аснихронные методы
        public async Task StartAsync()
        {
            while (true) {
                try {
                    if (settings.BeginHour <= DateTime.Now.Hour && DateTime.Now.Hour <= settings.EndHour) {
                        await CheckAsync();
                    }
                    Thread.Sleep(settings.RequestInterval);
                }
                catch (Exception ex) {
                    Log(ex.Message);
                }
            }
        }

        public async Task CheckAsync()
        {
            // Если с последней отправки прошло меньше MessagesInterval минут, то ничего не делаем
            if ((DateTime.Now - lastPhrasesSend) < settings.PhrasesInterval) {
                Log($"After the last sending has gone {Convert.ToInt32((DateTime.Now - lastPhrasesSend).TotalMinutes)} minutes.");
                return;
            }

            // Находим пользователя

            var users = await vkApi.Users.GetAsync(new long[] { settings.Id }, ProfileFields.Online | ProfileFields.OnlineMobile);
            var user = users.FirstOrDefault();

            // Если пользователь онлайн, то отправляем ему сообщение
            //if ((user.Online.HasValue && user.Online.Value) || (user.OnlineMobile.HasValue && user.OnlineMobile.Value)) {
            if ((user.Online != null && (bool)user.Online) || (user.OnlineMobile != null && (bool)user.OnlineMobile)) {

                string message = string.Empty;
                bool isSubjectSend = false;

                // Если с последней отправки сообщения о событии прошло больше AnnunciationMessagesInterval, 
                // то устанавливаем сообщение и флаг
                if ((DateTime.Now - lastSubjectSend) > settings.AnnunciationMessagesInterval) {

                    foreach (var subject in settings.Subjects) {
                        TimeSpan timeSpan = subject.Date - DateTime.Now;
                        message += GetAnnunciationMessage(subject.GenitiveName, timeSpan) + Environment.NewLine;
                    }

                    isSubjectSend = true;
                }
                else {
                    message = GetPhrase(times++);
                }

                // Отправка сообщения
                await vkApi.Messages.SendAsync(new MessagesSendParams {
                    Message = message,
                    UserId = user.Id
                });

                if (isSubjectSend) {
                    lastSubjectSend = DateTime.Now;
                }
                else {
                    lastPhrasesSend = DateTime.Now;
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
        #endregion

        private void Log(string logMessage)
        {
            logger?.Log(logMessage);
        }

        /// <summary>
        /// Выбирает фразу в зависимости от счетчика
        /// </summary>
        /// <param name="times">Счетчик кол-ва отправленных подряд сообщений</param>
        /// <returns></returns>
        private string GetPhrase(int times)
        {
            times = times < settings.Phrases.Length - 1 ? times : settings.Phrases.Length - 1;

            return settings.Phrases[times];
        }

        /// <summary>
        /// Выдает строку сообщения о событии
        /// </summary>
        /// <param name="name">Имя события</param>
        /// <param name="timeSpan">Кол-во оставшегося времени</param>
        /// <returns></returns>
        private string GetAnnunciationMessage(string name, TimeSpan timeSpan)
        {
            // Проверка формата вывода времени
            string time = string.Empty;

            if (days != null) {
                time += timeSpan.Days + " " + days.GetWord(timeSpan.Days);
            }

            if (hours != null) {
                time += " " + timeSpan.Hours + " " + hours.GetWord(timeSpan.Hours);
            }

            if (minutes != null) {
                time += " " + timeSpan.Minutes + " " + minutes.GetWord(timeSpan.Minutes);
            }

            return $"До {name.ToUpper()} {left.GetWord(timeSpan.Days)} {time}!";
        }
    }
}
