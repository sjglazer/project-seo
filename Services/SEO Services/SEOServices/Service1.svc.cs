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
using System.Reflection;

namespace SEOServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service1 : IKeywordService, IUserService
    {

        public Dictionary<string, Dictionary<string,List<KeywordStat>>> GetUserStats(string userId)
        {
            Dictionary<string, Dictionary<string,List<KeywordStat>>> ret = new Dictionary<string, Dictionary<string,List<KeywordStat>>>();
            User user = GetUser(userId);

            // loop through each of tthe users websites
            foreach(UrlKeywordSet set in user.urllKeywordSets)
            {
                string website = set.url;
                List<string> keywords = set.keywords;
                
                // loop through each keyword
                Dictionary<string, List<KeywordStat>> result = new Dictionary<string, List<KeywordStat>>();
                foreach(string keyword in keywords)
                {
                    List<KeywordStat> stats = CalculateKeywordStats(website , keyword);
                    result.Add(keyword, stats);
                }

                ret.Add(website, result);
            }
            return ret;
        }
        
        private List<KeywordStat> CalculateKeywordStats(string website , string keyword)
        {
            List<KeywordStat> ret = new List<KeywordStat>();
            List<KeywordCrawlResult> crawlResults = GetCrawlsByKeyword(keyword);
            
            if(crawlResults != null)
            {
                foreach(KeywordCrawlResult result in crawlResults)
                {
                    if (result != null)
                    {
                        KeywordStat stat = new KeywordStat();
                        stat.keyword = result.keyword;
                        stat.TimeStamp = result.TimeStamp;
                        int index = result.searchResults.FindIndex(x => x.StartsWith(website));
                        stat.position = index.ToString();
                        ret.Add(stat);
                    }
                }
            }
            return ret;
        }
        
        // IKeywordService Start
        public Dictionary<string, KeywordStats> GetKeywordStats(string userId)
        {
            
            Dictionary<string, KeywordStats> returnVal = new Dictionary<string, KeywordStats>();
            List<string> keywordList = GetUserKeywrodList(userId);
            if (keywordList != null && keywordList.Count > 0 )
            {
                // loop through each keyword
                foreach (string keyword in keywordList)
                {
                    if(!string.IsNullOrEmpty(keyword))
                    {
                        // Get all the crawl results for the keyword
                        KeywordStats current = new KeywordStats();
                        current.stats = new List<KeywordStat>();
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
                                    int index = crawlResult.searchResults.FindIndex(x => x.StartsWith("urlgoeshere"));
                                    stat.position = index.ToString();
                                    current.stats.Add(stat);
                                }
                            }

                            returnVal[keyword] = current;

                        }
                    }
                }
            }

            return returnVal;
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

        private List<string> GetUserKeywrodList(string userId)
        {
            List<string> ret = new List<string>();
            User user = GetUser(userId);
            if(user != null && user.urllKeywordSets.Count() > 0)
            {
                foreach(UrlKeywordSet set in user.urllKeywordSets)
                {
                    ret = ret.Union(set.keywords).ToList();
                }
            }
            return ret;
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
                    foreach( Dictionary<string, object> pair in value)
                    {
                        object[] keywords = (object[])pair["keywords"];
                        List<string> fields = keywords.Select(i => i.ToString()).ToList();
                        ret.keywords = ret.keywords.Union(fields).ToList();
                    }
                }
            }
           
            return ret;
        }
        // IKeywordService End

        // IUserService Start
        public bool AddUser(User user)
        {
            var key = user.id;
            var client = CouchbaseManager.Instance;
            var result = client.StoreJson(StoreMode.Add, key, user);
            return result;
        }

        public bool UpdateUser(User user)
        {
            var key = user.id;
            var client = CouchbaseManager.Instance;
            var result = client.StoreJson(StoreMode.Replace, key, user);
            return result;
        }

        public User GetUser(string id)
        {
            var client = CouchbaseManager.Instance;
            var user = client.GetJson<User>(id);
            return user;
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
