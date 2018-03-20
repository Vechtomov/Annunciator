using System;
using VkAnnunciator.Log;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EgeAnnunciator
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = null;
            try {
                string annunciatorSettingsJson = File.ReadAllText("AnnunciatorSettings.txt");
                string vkSettingsJson = File.ReadAllText("VkSettings.txt");

                JObject annunciatorSettingsObject = JObject.Parse(annunciatorSettingsJson);
                JObject vkSettingsObject = JObject.Parse(vkSettingsJson);

                AnnunciatorSettings annunciatorSettings = JsonConvert.DeserializeObject<AnnunciatorSettings>(annunciatorSettingsObject.ToString());
                VkSettings vkSettings = JsonConvert.DeserializeObject<VkSettings>(vkSettingsObject.ToString());

                logger = InitializeFileLogger(annunciatorSettings.Id.ToString());
                VkAnnunciator annunciator = new VkAnnunciator(annunciatorSettings, vkSettings, logger);
                
                annunciator.Start();
            }
            catch(Exception ex) {
                logger?.Log(ex.Message);
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