using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SEOServices
{
    public class WebUtils
    {

        public static List<string> GetGoogleResults(string url)
        {
            string html = DownloadString(url);
            return ParseGoogleResults(html);
        }
        
        private static List<string> ParseGoogleResults(string html)
        {
            List<string> domains = new List<string>();

            HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.Load(new StringReader(html));
            if (htmlDoc != null)
            {
                var nodes = htmlDoc.DocumentNode.SelectNodes(SEOServices.Properties.Resources.GoogleXPath);
                foreach (var node in nodes)
                {
                    var domain = node.Attributes["href"].Value;

                    int index = domain.IndexOf("&amp");
                    if (index > 0)
                    {
                        domain = domain.Substring(7, index - 7);
                    }
                    
                    if(domain.StartsWith("http"))
                        domains.Add(domain);
                    
                }
            }
            return domains;
        }
        
        private static string DownloadString(string address)
        {
            WebClient client = new WebClient();
            string reply = client.DownloadString(address);
            return reply;
        }
    }
}