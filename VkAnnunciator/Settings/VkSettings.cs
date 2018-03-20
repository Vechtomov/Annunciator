using Newtonsoft.Json;

namespace EgeAnnunciator
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct VkSettings
    {
        [JsonProperty("appId")]
        public ulong AppId { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}