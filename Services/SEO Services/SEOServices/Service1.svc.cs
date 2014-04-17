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
    public class Service1 : IKeywordService
    {
        public List<string> GetKeywordResults(string lang, int num, string searchTerm, int start, string country)
        {
            string url = string.Format(SEOServices.Properties.Resources.GoogleSearch, lang, num, searchTerm, start, country);
            return WebUtils.GetGoogleResults(url);
        }

        public bool AddKeyword(string keyword)
        {
            var client = new CouchbaseClient();

            var newKeyword = new Keywords
            {
                Keyword = "test"
            };
            
            var key = "Stephen1";
            var result = client.StoreJson(StoreMode.Add, key, newKeyword);

            return true;
        }
    }
}
