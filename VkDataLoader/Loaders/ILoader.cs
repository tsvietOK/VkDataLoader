using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VkDataLoader.Loaders
{
    internal interface ILoader
    {
        Task LoadAsync(HttpClient client, string url, int i);
    }
}
