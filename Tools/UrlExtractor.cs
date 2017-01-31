using System.Collections.Generic;
using System.IO;

namespace Tools
{
    public class UrlExtractor
    {

        public static IEnumerable<Url> ExtractUrls(string filepath)
        {
            var urls = new List<Url>();
            if (filepath == null)
            {
                return urls;
            }

            using (StreamReader reader = new StreamReader(filepath))
            {
                string line;
                int position = 1;
                while ((line = reader.ReadLine()) != null)
                {
                    urls.Add(new Url(line, position));
                    position++;
                }
            }

            return urls;
        }
    }
}
