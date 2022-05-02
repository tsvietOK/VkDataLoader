﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VkDataLoader
{
    public class VkDataProcessorFactory
    {
        private const string configFileName = "config.json";
        private readonly string configFilePath;

        private VkDataProcessor? dataProcessor;

        public VkDataProcessorFactory(string folderPath)
        {
            configFilePath = Path.Combine(folderPath, configFileName);
            IsVkFolder = File.Exists(Path.Combine(folderPath, "index.html"));
            if (!TryLoadConfigurationAsync(folderPath).GetAwaiter().GetResult())
            {
                dataProcessor = IsVkFolder ? new VkDataProcessor(folderPath, this) : null;
            }
        }

        public bool IsVkFolder { get; set; }

        public VkDataProcessor? GetVkDataProcessor() => dataProcessor;

        public void SaveConfiguration()
        {
            JsonSerializer serializer = new();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using StreamWriter sw = new(configFilePath);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, dataProcessor?.Parser);
        }

        public async Task<bool> TryLoadConfigurationAsync(string folderPath)
        {
            if (File.Exists(configFilePath))
            {
                using var reader = File.OpenText(configFilePath);
                string config = await reader.ReadToEndAsync();
                LinksParser? linksParser;
                try
                {
                    linksParser = JsonConvert.DeserializeObject<LinksParser>(config);
                }
                catch (Exception)
                {
                    File.Delete(configFilePath);
                    return false;
                }

                if (linksParser is not null)
                {
                    dataProcessor = new VkDataProcessor(linksParser, folderPath, this);
                    return true;
                }
            }
            return false;
        }
    }
}
