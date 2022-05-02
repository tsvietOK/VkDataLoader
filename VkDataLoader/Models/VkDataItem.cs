using VkDataLoader.Enums;

namespace VkDataLoader.Models
{
    public class VkDataItem
    {
        public string Url { get; set; }

        public VkDataType DataType { get; set; }

        public bool IsDownloaded { get; set; }

        public bool IsDownloadFailed { get; set; }
    }
}
