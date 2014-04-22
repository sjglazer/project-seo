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
        bool CrawlKeyword(string keyword);

        [OperationContract]
        Dictionary<string,KeywordStats> GetKeywordStats(string userId);

        [OperationContract]
        Dictionary<string, Dictionary<string, List<KeywordStat>>> GetUserStats(string userId);
    }

    [ServiceContract]
    public interface IUserService
    {
        
        [OperationContract]
        bool AddUser(User user);

        [OperationContract]
        bool UpdateUser(User user);

        [OperationContract]
        User GetUser(string id);
        
        [OperationContract]
        WebsiteInfo GetWebsiteInfo(string userId);
    }

    [DataContract]
    public class User
    {

        [DataMember]
        [JsonProperty("id")]
        public string id { get; set; }
        
        [DataMember]
        [JsonProperty("urlLimit")]
        public int urlLimit { get; set; }

        [DataMember]
        [JsonProperty("keywordlLimit")]
        public int keywordlLimit { get; set; }

        [DataMember]
        [JsonProperty("urllKeywordSets")]
        
        public List<UrlKeywordSet> urllKeywordSets { get; set; }

        [JsonProperty("type")]
        public string Type
        {
            get { return "user"; }
        }
    }

    [DataContract]
    public class UrlKeywordSet
    {
        [DataMember]
        [JsonProperty("url")]
        public string url { get; set; }

        [DataMember]
        [JsonProperty("keywords")]
        public List<string> keywords { get; set; }
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
