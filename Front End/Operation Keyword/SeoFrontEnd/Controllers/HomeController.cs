using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeoFrontEnd.Models;
using SeoFrontEnd;

namespace SeoFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

        //    var model = new HomeModel();
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_DomainList", model);
        //    }

        //    return View(model);
        //}

        public ActionResult Index(HomeModel model)
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            if (model.domains == null)
            {
                model.domains = new List<string>();
            }

            
            
            if (Request.IsAjaxRequest())
            {
                var country = Request.Params["country_code"];
                var lang = Request.Params["lang_code"];
                var searchTerm = Request.Params["name"];

                var request = new ServiceReference1.GetKeywordResultsRequest();
                request.country = country;
                request.lang = lang;
                request.num = 100;
                request.searchTerm = searchTerm;
                request.start = 0;
                var list = new ServiceReference1.KeywordServiceClient().GetKeywordResults(request);
                model.domains = list.GetKeywordResultsResult.ToList();
                return PartialView("_DomainList", model);
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
