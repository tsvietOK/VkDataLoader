namespace VkDataLoader.Loaders
{
    internal interface ILoader
    {
        Task<bool> TryLoadAsync(HttpClient client, string url, int suffix, string path);
    }
}
