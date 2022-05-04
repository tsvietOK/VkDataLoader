namespace VkDataLoader.Loaders
{
    internal class ImageLoader : ILoader
    {
        public ImageLoader(string applicationFolderPath)
        {
            ApplicationFolder = applicationFolderPath;

            if (!Directory.Exists(GetFolderName()))
            {
                Directory.CreateDirectory(GetFolderName());
            }
        }

        public string ApplicationFolder { get; }

        public string GetFolderName() => Path.Combine(ApplicationFolder, "images");

        public async Task<bool> TryLoadAsync(HttpClient client, string url, int suffix)
        {
            try
            {
                await using var stream = await client.GetStreamAsync(url);
                string downloadPath = Path.Combine(GetFolderName(), $"vk_image_{suffix}.jpg");
                await using var fileStream = new FileStream(downloadPath, FileMode.OpenOrCreate);
                await stream.CopyToAsync(fileStream);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
