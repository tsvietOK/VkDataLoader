namespace VkDataLoader.Loaders
{
    internal class ImageLoader : ILoader
    {
        public async Task<bool> TryLoadAsync(HttpClient client, string url, int suffix, string path)
        {
            try
            {
                using var stream = await client.GetStreamAsync(url);
                string downloadPath = Path.Combine(path, $"images/vk_image_{suffix}.jpg");
                using var fileStream = new FileStream(downloadPath, FileMode.OpenOrCreate);
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
