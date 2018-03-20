using Newtonsoft.Json;
using System;

namespace EgeAnnunciator
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct AnnunciatorSettings
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("messagesInterval")]
        public int MessagesInterval { get; set; }

        [JsonProperty("egeMessagesInterval")]
        public int EgeMessagesInterval { get; set; }

        [JsonProperty("beginHour")]
        public int BeginHour { get; set; }

        [JsonProperty("endHour")]
        public int EndHour { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }

        [JsonProperty("phrases")]
        public string[] Phrases { get; set; }

        [JsonProperty("subjects")]
        public Subject[] Subjects { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public struct Subject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("genitiveName")]
        public string GenitiveName { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}