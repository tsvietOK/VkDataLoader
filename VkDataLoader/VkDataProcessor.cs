using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VkDataLoader.Models;

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

        public async Task ParseItems(List<string> itemsToLoad)
        {
            Parser.Reset();
            await Parser.ParseAsync(itemsToLoad);
            vkDataProcessorFactory.SaveConfiguration();
        }
    }
}