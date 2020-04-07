using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FDCrawler
{
    class Settings
    {
        public string[] ethUrls { get; }
        public string mongodbUrl { get; }
        public string mongodbDatabase { get; }
        public int waitingTime { get; }

        private static Settings ins;
        public static Settings Ins
        {
            get
            {
                JsonConfigurationSource js = new JsonConfigurationSource();
                js.Path = "config.json";
                if (ins == null)
                    ins = new Settings(new ConfigurationBuilder().AddJsonFile("config.json").Build());
                return ins;
            }
        }

        public Settings(IConfiguration section)
        {
            IConfigurationSection section_urls = section.GetSection("ethUrls");
            if(section_urls.Exists())
                this.ethUrls = section_urls.GetChildren().Select(p => p.Value).ToArray();
            this.mongodbUrl = section.GetSection("mongodbUrl").Value;
            this.mongodbDatabase = section.GetSection("mongodbDatabase").Value;
            this.waitingTime =int.Parse(section.GetSection("waitingTime").Value);
        }
    }
}
