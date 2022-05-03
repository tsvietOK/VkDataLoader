namespace VkDataLoader.Loaders
{
    internal class ImageLoader : ILoader
    {
        public async Task<bool> TryLoadAsync(HttpClient client, string url, int suffix)
        {
            try
            {
                using var stream = await client.GetStreamAsync(url);
                using var fileStream = new FileStream($"./images/vk_image_{suffix}.jpg", FileMode.OpenOrCreate);
                await stream.CopyToAsync(fileStream);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
