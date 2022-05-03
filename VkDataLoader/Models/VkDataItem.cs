using VkDataLoader.Enums;

namespace VkDataLoader.Models
{
    public class VkDataItem
    {
        public string Url { get; set; }

        public VkDataType DataType { get; set; }

        public VkDataDownloadStatus DownloadStatus { get; set; }
    }
}
