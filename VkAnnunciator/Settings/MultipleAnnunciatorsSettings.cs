using Newtonsoft.Json;

namespace VkAnnunciator.Settings
{
    /// <summary>
    /// Массив сигнализаторов
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct MultipleAnnunciatorsSettings
    {
        [JsonProperty("annunciators")]
        public AnnunciatorSettings[] Annunciators { get; set; }
    }
}
