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
