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
        public void CrawlKeyword(string keyword)
        {

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
