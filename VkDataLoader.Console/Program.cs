using McMaster.Extensions.CommandLineUtils;

namespace VkDataLoader.Console
{
    public class Program
    {
        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Path to the 'messages' folder.")]
        public string MessageFolderPath { get; } = @"C:\Users\tsvet\Downloads\Archive\messages";

        [Option(Description = "What you want to download.")]
        public string ItemsToLoad { get; } = "images";

        private void OnExecute()
        {
            VkDataProcessor data = new(MessageFolderPath);
            data.ParseItems(ItemsToLoad);
        }
    }

}