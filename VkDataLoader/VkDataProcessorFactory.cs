using Newtonsoft.Json;
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
            if (!TryLoadConfiguration(folderPath))
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

        public bool TryLoadConfiguration(string folderPath)
        {
            if (File.Exists(configFilePath))
            {
                using var stream = File.OpenText(configFilePath);
                using var reader = new JsonTextReader(stream);
                LinksParser? linksParser;
                try
                {
                    var convert = new JsonSerializer();
                    linksParser = convert.Deserialize<LinksParser>(reader);
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
