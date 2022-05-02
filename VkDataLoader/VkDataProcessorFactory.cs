using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VkDataLoader
{
    public class VkDataProcessorFactory
    {
        public bool IsVkFolder { get; set; }

        public VkDataProcessorFactory(string folderPath)
        {
            IsVkFolder = File.Exists(Path.Combine(folderPath, "index.html"));
        }

        public VkDataProcessor? GetVkDataProcessor(string folderPath)
        {
            return IsVkFolder ? new VkDataProcessor(folderPath) : null;
        }
    }
}
