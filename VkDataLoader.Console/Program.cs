using McMaster.Extensions.CommandLineUtils;

namespace VkDataLoader.Console
{
    public class Program
    {
        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Path to the 'messages' folder.")]
        public string MessageFolderPath { get; } = @"C:\Users\tsvet\Downloads\Archive\messages";

        private void OnExecute()
        {
            VkData data = new(MessageFolderPath);
            data.Load();
        }
    }

}