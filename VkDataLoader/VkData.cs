namespace VkDataLoader
{
    public class VkData
    {
        public VkData(string messageFolderPath)
        {
            LinksParser linksParser = new(messageFolderPath);
        }

        public void Load()
        {
            Console.WriteLine("Hello");
        }
    }
}