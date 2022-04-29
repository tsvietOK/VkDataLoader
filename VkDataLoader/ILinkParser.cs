namespace VkDataLoader
{
    internal interface ILinkParser
    {
        List<string> GetLinksFromHtml(string html);
    }
}
