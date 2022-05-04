using System.Diagnostics;

namespace VkDataLoader
{
    public class VkDataProcessor
    {
        private readonly VkDataProcessorFactory vkDataProcessorFactory;

        internal VkDataProcessor(LinksParser parser, string vkFolderPath, VkDataProcessorFactory processorFactory) : this(processorFactory)
        {
            Parser = parser;
            Parser.SetFolderPath(vkFolderPath);
        }

        internal VkDataProcessor(string vkFolderPath, VkDataProcessorFactory processorFactory) : this(processorFactory)
        {
            Parser = new(vkFolderPath);
        }

        private VkDataProcessor(VkDataProcessorFactory processorFactory)
        {
            vkDataProcessorFactory = processorFactory;
        }

        public LinksParser Parser { get; set; }

        public LinksLoader Loader { get; set; }

        public async Task ParseItems(List<string> itemsToLoad)
        {
            Parser.Reset();
            await Parser.ParseAsync(itemsToLoad);
            vkDataProcessorFactory.SaveConfiguration();
        }

        public async Task LoadParsedItems()
        {
            Loader = new LinksLoader(vkDataProcessorFactory);
            await Loader.LoadAsync(Parser.VkDataItems);
        }

        public void OpenFolder()
        {
            if (Directory.Exists(vkDataProcessorFactory.ApplicationFolderPath))
            {
                Process.Start("explorer.exe", vkDataProcessorFactory.ApplicationFolderPath);
            }
        }
    }
}