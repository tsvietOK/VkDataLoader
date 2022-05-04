using Newtonsoft.Json;

namespace VkDataLoader
{
    public class VkDataProcessorFactory
    {
        private const string CONFIG_FILE_NAME = "config.json";
        private const string APPLICATION_FOLDER = "VkDataLoader";
        private readonly string configFilePath;

        private VkDataProcessor? dataProcessor;

        public VkDataProcessorFactory(string folderPath)
        {
            ApplicationFolderPath = Path.Combine(folderPath, APPLICATION_FOLDER);
            configFilePath = Path.Combine(ApplicationFolderPath, CONFIG_FILE_NAME);
            IsVkFolder = File.Exists(Path.Combine(folderPath, "index.html"));
            if (!IsVkFolder)
            {
                return;
            }

            if (!Directory.Exists(Path.Combine(folderPath, APPLICATION_FOLDER)))
            {
                Directory.CreateDirectory(Path.Combine(folderPath, APPLICATION_FOLDER));
            }

            IsConfigurationLoaded = TryLoadConfiguration(folderPath);
            if (!IsConfigurationLoaded)
            {
                dataProcessor = IsVkFolder ? new VkDataProcessor(folderPath, this) : null;
            }
        }

        public bool IsVkFolder { get; set; }

        public bool IsConfigurationLoaded { get; set; }

        public string ApplicationFolderPath { get; private set; }

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
