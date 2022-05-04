namespace VkDataLoader.Loaders
{
    internal interface ILoader
    {
        string GetFolderName();

        Task<bool> TryLoadAsync(HttpClient client, string url, int suffix);
    }
}
