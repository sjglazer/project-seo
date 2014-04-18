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

namespace SEOServices
{
    public class Service1 : IKeywordService, IUserService
    {
        public List<string> GetKeywordResults(string lang, int num, string searchTerm, int start, string country)
        {
            string url = string.Format(SEOServices.Properties.Resources.GoogleSearch, lang, num, searchTerm, start, country);
            return WebUtils.GetGoogleResults(url);
        }

        public bool AddWebsiteInfo(string userId, string url, List<string> keywords)
        {
            var client = new CouchbaseClient();
            var newKeyword = new UserWebsiteProperties
            {
                url = url,
                keywords = keywords
            };

            var key = userId;
            var result = client.StoreJson(StoreMode.Add, key, newKeyword);

            return result;
        }
    }

}
