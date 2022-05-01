using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VkDataLoader.Loaders
{
    internal class ImageLoader : ILoader
    {
        public async Task LoadAsync(HttpClient client, string url, int i)
        {
            using var stream = await client.GetStreamAsync(url);
            using var fileStream = new FileStream($"./images/vk_image_{i}.jpg", FileMode.OpenOrCreate);
            stream.CopyTo(fileStream);
        }
    }
}
