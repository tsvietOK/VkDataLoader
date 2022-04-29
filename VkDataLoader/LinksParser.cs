using HtmlAgilityPack;

namespace VkDataLoader
{
    public class LinksParser : ILinkParser
    {
        private readonly string _messageFolderPath;
        public LinksParser(string messageFolderPath)
        {
            _messageFolderPath = messageFolderPath;
        }

        public void Parse()
        {
            List<string> finalList = new();
            string[] files = Directory.GetFiles(_messageFolderPath, "*.html", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                //Console.WriteLine($"Processing file:{file}");
                var text = File.ReadAllText(file);
                var linksInFile = GetLinksFromHtml(text);
                finalList.AddRange(linksInFile);
            }


            using TextWriter tw = new StreamWriter("SavedList.txt");
            foreach (string s in finalList)
            {
                tw.WriteLine(s);
            }
        }

        public List<string> GetLinksFromHtml(string html)
        {
            HtmlDocument htmlSnippet = new();
            htmlSnippet.LoadHtml(html);

            List<string> hrefTags = new();
            if (htmlSnippet.DocumentNode.SelectNodes($"//div[@class='attachment']/a[@class='attachment__link']") != null)
            {
                foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes($"//div[@class='attachment']/a[@class='attachment__link']"))
                {
                    HtmlAttribute att = link.Attributes["href"];
                    var href = att.Value;
                    if (href.Contains("userapi"))
                    {
                        hrefTags.Add(att.Value);
                    }
                }
            }

            return hrefTags;
        }
    }
}