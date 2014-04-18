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
    }

    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        bool AddWebsiteInfo(string userId, string url, List<string> keywords);
    }
}
