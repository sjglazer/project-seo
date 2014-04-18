using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SEOServices
{
   
    [ServiceContract]
    public interface IKeywordService
    {
        [OperationContract]
        List<string> GetKeywordResults(string lang, int num, string searchTerm, int start, string country);

        [OperationContract]
        KeywordList GetAllKeywords();

        [OperationContract]
        void CrawlKeyword(string keyword);
    }

    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        bool AddWebsiteInfo(string userId, string url, List<string> keywords);

        [OperationContract]
        WebsiteInfo GetWebsiteInfo(string userId);
    }

    [DataContract]
    public class WebsiteInfo
    {
        [DataMember]
        [JsonProperty("url")]
        public string url { get; set; }

        [DataMember]
        [JsonProperty("keywords")]
        public List<string> keywords { get; set; }
       
    }

    [DataContract]
    public class KeywordList
    {
        [DataMember]
        [JsonProperty("keywords")]
        public List<string> keywords { get; set; }

    }
}
