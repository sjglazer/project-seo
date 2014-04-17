using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.IO;


namespace SEOServices
{
    public class Service1 : IKeywordService
    {
        public List<string> GetKeywordResults(string lang, int num, string searchTerm, int start, string country)
        {
            string url = string.Format(SEOServices.Properties.Resources.GoogleSearch, lang, num, searchTerm, start, country);
            return WebUtils.GetGoogleResults(url);
        }
    }
}
