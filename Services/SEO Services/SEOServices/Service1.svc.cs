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
        public Dictionary<string, KeywordStats> GetKeywordStats(string userId)
        {
            // Get the users domain and keywords
            Dictionary<string, KeywordStats> returnVal = new Dictionary<string, KeywordStats>();
            WebsiteInfo websiteInfo = GetWebsiteInfo(userId);
            if(websiteInfo != null && websiteInfo.keywords != null && websiteInfo.keywords.Count > 0 && !string.IsNullOrEmpty(websiteInfo.url))
            {
                // loop through each keyword
                foreach(string keyword in websiteInfo.keywords)
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
                                    int index = crawlResult.searchResults.FindIndex(x => x.StartsWith(websiteInfo.url));
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

        public bool AddWebsiteInfo(string userId, string url, List<string> keywords)
        {
            
            WebsiteInfo info = GetWebsiteInfo(userId);
            if(info == null)
            {
                //return AddUser(userId, url, keywords);
                
                //var newKeyword = new User
                //{
                //    url = url,
                //    keywords = keywords
                //};

                //var key = userId;
                //var client = CouchbaseManager.Instance;
                //var result = client.StoreJson(StoreMode.Add, key, newKeyword);
                //return result;
            }
            else
            {
               //return AddUser(userId, url, keywords);
                
                //var newKeyword = new User();
                //if (!string.IsNullOrEmpty(url))
                //{
                //    newKeyword.url = url;
                //} 
                //else
                //{
                //    newKeyword.url = info.url;
                //}

                //if (keywords != null)
                //{
                //    newKeyword.keywords = keywords;
                //}
                //else
                //{
                //    newKeyword.keywords = info.keywords;
                //}

                //var key = userId;
                //var client = CouchbaseManager.Instance;
                //var result = client.StoreJson(StoreMode.Replace, key, newKeyword);
                //return result;
               
            }
            return true;
        }

        //private bool AddUser(string userId, string url, List<string> keywords)
        //{
        //    User user = new User();
        //    UrlKeywordSet urlKeywordSet = new UrlKeywordSet();
            
        //    if(!string.IsNullOrEmpty(url))
        //    {
        //        urlKeywordSet.url = url;

        //        if (keywords != null)
        //        {
        //            urlKeywordSet.keywords = keywords;
        //        }

        //        user.urllKeywordSets = new List<UrlKeywordSet>();
        //        user.urllKeywordSets.Add(urlKeywordSet);
        //    }

        //    var key = userId;
        //    var client = CouchbaseManager.Instance;
        //    var result = client.StoreJson(StoreMode.Add, key, user);
        //    return result;
        //}

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
