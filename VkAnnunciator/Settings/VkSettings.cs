using Newtonsoft.Json;

namespace VkAnnunciator.Settings
{
    /// <summary>
    /// Настройки API VK
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct VkSettings
    {
        /// <summary>
        /// Id приложения
        /// </summary>
        [JsonProperty("appId")]
        public ulong AppId { get; set; }

        /// <summary>
        /// Логин бота
        /// </summary>
        [JsonProperty("login")]
        public string Login { get; set; }

        /// <summary>
        /// Пароль бота
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}