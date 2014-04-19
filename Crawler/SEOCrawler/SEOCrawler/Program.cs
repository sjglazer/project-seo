using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEOCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the list of keywords to crawl
            var keywords = new SEOService.KeywordServiceClient().GetAllKeywords();

            foreach(var keyword in keywords.keywords)
            {
                if(!string.IsNullOrEmpty(keyword))
                {
                    bool result = new SEOService.KeywordServiceClient().CrawlKeyword(keyword);
                }
                    
            }
        }
    }
}
