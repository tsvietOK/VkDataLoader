namespace VkDataLoader.Loaders
{
    internal interface ILoader
    {
        Task LoadAsync(HttpClient client, string url, int i);
    }
}
