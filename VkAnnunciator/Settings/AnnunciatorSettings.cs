using Newtonsoft.Json;
using System;

namespace VkAnnunciator.Settings
{
    /// <summary>
    /// Настройки сигнализатора
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct AnnunciatorSettings
    {
        /// <summary>
        /// Id пользователя в Вконтакте
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Интервал между фразами
        /// </summary>
        [JsonProperty("phrasesInterval")]
        public TimeSpan PhrasesInterval { get; set; }

        /// <summary>
        /// Интервал между сигнализирующими сообщениями
        /// </summary>
        [JsonProperty("annunciationMessagesInterval")]
        public TimeSpan AnnunciationMessagesInterval { get; set; }

        /// <summary>
        /// Формат вывода оставшегося до события времени:
        /// d - выводить только оставшиеся дни
        /// dh - выводить дни и часы
        /// </summary>
        [JsonProperty("annunciationFormat")]
        public string AnnunciationFormat { get; set; }

        /// <summary>
        /// Время в часах, после которого нужно посылать сообщения
        /// Например: 9 - сообщения будут посылаться после 9:00
        /// </summary>
        [JsonProperty("beginHour")]
        public int BeginHour { get; set; }

        /// <summary>
        /// Время в часах, до которого нужно посылать сообщения
        /// Например: 20 - сообщения будут посылаться до 20:00
        /// </summary>
        [JsonProperty("endHour")]
        public int EndHour { get; set; }

        /// <summary>
        /// Интервал в миллисекундах между запросами
        /// </summary>
        [JsonProperty("requestInterval")]
        public int RequestInterval { get; set; }

        /// <summary>
        /// Массив фраз
        /// Сначала посылается первая фраза, затем, если ползователь не вышел из сети через 
        /// интервал времени PhrasesInterval, посылается следующая фраза, и так далее, пока пользователь
        /// не выйдет из сети или фразы не закончатся. Затем опять отправляется первая фраза
        /// </summary>
        [JsonProperty("phrases")]
        public string[] Phrases { get; set; }


        /// <summary>
        /// Массив сигнализируемых сщбытий
        /// </summary>
        [JsonProperty("subjects")]
        public Subject[] Subjects { get; set; }
    }

    /// <summary>
    /// Событие, о котором нужно уведомлять
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct Subject
    {
        /// <summary>
        /// Имя события
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Имя события в родительном падеже
        /// </summary>
        [JsonProperty("genitiveName")]
        public string GenitiveName { get; set; }

        /// <summary>
        /// Дата, когда произойдет событие
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}