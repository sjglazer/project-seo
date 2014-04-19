using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEOServices
{
    public class UserWebsiteProperties
    {
        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("keywords")]
        public List<string> keywords { get; set; }

        [JsonProperty("type")]
        public string type = "UserWebsiteProperties";
    }

   
    public class KeywordCrawlResult
    {
        [JsonProperty("searchResults")]
        public List<string> searchResults { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("keyword")]
        public string keyword { get; set; }

        [JsonProperty("type")]
        public string type = "KeywordCrawResult";
    }

    public class KeywordStats
    {
        [JsonProperty("stats")]
        public List<KeywordStat> stats { get; set; }

        [JsonProperty("type")]
        public string type = "KeywordStat";
    }

    public class KeywordStat
    {
        [JsonProperty("keyword")]
        public string keyword { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("position")]
        public string position { get; set; }

        [JsonProperty("type")]
        public string type = "KeywordStat";
    }
}