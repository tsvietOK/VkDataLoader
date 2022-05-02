using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VkDataLoader
{
    public class VkDataProcessorFactory
    {
        private readonly VkDataProcessor? dataProcessor;

        public VkDataProcessorFactory(string folderPath)
        {
            IsVkFolder = File.Exists(Path.Combine(folderPath, "index.html"));
            dataProcessor = IsVkFolder ? new VkDataProcessor(folderPath) : null;
        }

        public bool IsVkFolder { get; set; }

        public VkDataProcessor? GetVkDataProcessor() => dataProcessor;
    }
}
