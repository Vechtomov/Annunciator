using System;
using VkAnnunciator.Log;
using VkAnnunciator.Settings;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Annunciator
{
    class Program
    {
        static ILogger applicationLogger = InitializeFileLogger("Application");

        static void Main(string[] args)
        {
            try {
                // Считываем файлы конфигурации
                string annunciatorsJson = File.ReadAllText("AnnunciatorsSettings.txt");
                string vkSettingsJson = File.ReadAllText("VkSettings.txt");

                // Преобразуем в JObject
                JObject annunciatorsObject = JObject.Parse(annunciatorsJson);
                JObject vkSettingsObject = JObject.Parse(vkSettingsJson);

                // Десериализуем объекты
                MultipleAnnunciatorsSettings annunciatorsSettings = JsonConvert.DeserializeObject<MultipleAnnunciatorsSettings>(annunciatorsObject.ToString());
                VkSettings vkSettings = JsonConvert.DeserializeObject<VkSettings>(vkSettingsObject.ToString());

                // Запускаем параллельно сигнализаторы
                Parallel.ForEach(annunciatorsSettings.Annunciators, a => {
                    ILogger logger = InitializeFileLogger(a.Id.ToString());
                    VkAnnunciator annunciator = new VkAnnunciator(a, vkSettings, logger);
                    applicationLogger.Log(a.Id + " created " + DateTime.Now.ToLongTimeString());
                    annunciator.Start();
                });
            }
            catch (Exception ex) {
                applicationLogger?.Log(ex.Message);
            }
        }


        static ILogger InitializeFileLogger(string name)
        {
            return new FileLogger($"{name}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}", "Log");
        }

        static ILogger InitializeConsoleLogger()
        {
            return new ConsoleLogger();
        }
    }
}