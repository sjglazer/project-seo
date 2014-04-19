using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Couchbase;
using Couchbase.Extensions;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;
using System.ServiceModel.Activation;

namespace SEOServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service1 : IKeywordService, IUserService
    {

        // IKeywordService Start
        public KeywordStats GetKeywordStats(string userId)
        {
            // Get the users domain and keywords
            KeywordStats ret = new KeywordStats();
            ret.stats = new List<KeywordStat>();
            WebsiteInfo websiteInfo = GetWebsiteInfo(userId);
            if(websiteInfo != null && websiteInfo.keywords != null && websiteInfo.keywords.Count > 0 && !string.IsNullOrEmpty(websiteInfo.url))
            {
                // loop through each keyword
                foreach(string keyword in websiteInfo.keywords)
                {
                    if(!string.IsNullOrEmpty(keyword))
                    {
                        // Get all the crawl results for the keyword
                        List<KeywordCrawlResult> results = GetCrawlsByKeyword(keyword);
                        if(results != null && results.Count > 0)
                        {
                            foreach(KeywordCrawlResult crawlResult in results)
                            {
                                if(crawlResult != null)
                                {
                                    KeywordStat stat = new KeywordStat();
                                    stat.keyword = crawlResult.keyword;
                                    stat.TimeStamp = crawlResult.TimeStamp;
                                    int index = crawlResult.searchResults.FindIndex(x => x.StartsWith(websiteInfo.url));
                                    stat.position = index.ToString();
                                    ret.stats.Add(stat);
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        
        private List<KeywordCrawlResult> GetCrawlsByKeyword(string keyword)
        {
            var client = CouchbaseManager.Instance;
            var view = client.GetView <KeywordCrawlResult>("crawlbykeyword", "crawlbykeyword", true).StartKey(keyword).EndKey(keyword);
            var count = view.Count();
            var list =  new List<KeywordCrawlResult>();

            if (count > 0)
            {
                foreach (var row in view)
                {
                    list.Add(row);
                }
            }

            return list;
        }
        
        
        public bool CrawlKeyword(string keyword)
        {
            string url = string.Format(SEOServices.Properties.Resources.GoogleSearch, string.Empty, 100, keyword, 0, string.Empty);
            List<string> crawlUrls = WebUtils.GetGoogleResults(url);

            var client = CouchbaseManager.Instance;
            var keyIndex = client.Increment("keywordCrawResultCounter", 100, 1);
            
            string key = string.Format("keyword_{0}", keyIndex);
            KeywordCrawlResult result = new KeywordCrawlResult();
            result.TimeStamp = DateTime.Now;
            result.searchResults = crawlUrls;
            result.keyword = keyword;

            var ret = client.StoreJson(StoreMode.Add, key, result);
            return ret;
        }
        
        public List<string> GetKeywordResults(string lang, int num, string searchTerm, int start, string country)
        {
            string url = string.Format(SEOServices.Properties.Resources.GoogleSearch, lang, num, searchTerm, start, country);
            return WebUtils.GetGoogleResults(url);
        }

        public KeywordList GetAllKeywords()
        {
            var client = CouchbaseManager.Instance;
            var view = client.GetView("keywords", "keywords", true);
            var count = view.Count();

            var ret = new KeywordList();
            ret.keywords = new List<string>();

            if(count > 0)
            {
                foreach (var row in view)
                {
                    object[] value = row.Info["value"] as object[];
                    List<string> list = Array.ConvertAll<object, string>(value, ConvertObjectToString).ToList();
                    ret.keywords = ret.keywords.Union(list).ToList();
                    
                }
            }
           
            return ret;
        }

        // IKeywordService End

        // IUserService Start

        public bool AddWebsiteInfo(string userId, string url, List<string> keywords)
        {
            
            // save the user's properties: domain + keywords
            var client = CouchbaseManager.Instance;
            var newKeyword = new UserWebsiteProperties
            {
                url = url,
                keywords = keywords
            };

            var key = userId;
            var result = client.StoreJson(StoreMode.Set, key, newKeyword);
            return result;
        }

        public WebsiteInfo GetWebsiteInfo(string userId)
        {
            var client = CouchbaseManager.Instance;
            var websiteInfo = client.GetJson<WebsiteInfo>(userId);
            return websiteInfo;
        }

        // IUserService End

        private string ConvertObjectToString(object obj)
        {
            return (obj == null) ? string.Empty : obj.ToString();
        }
    }

    public static class CouchbaseManager
    {
        private readonly static CouchbaseClient _instance;

        static CouchbaseManager()
        {
            _instance = new CouchbaseClient();
        }

        public static CouchbaseClient Instance { get { return _instance; } }
    }

}
