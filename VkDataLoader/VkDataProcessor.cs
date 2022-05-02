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
        public LinksParser Parser { get; set; }

        internal VkDataProcessor(string vkFolderPath)
        {
            Parser = new(vkFolderPath);
        }

        public async Task ParseItems(string itemsToLoad)
        {
            await Parser.ParseAsync(itemsToLoad);
        }
    }
}