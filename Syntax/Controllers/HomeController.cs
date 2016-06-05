using Syntax.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Syntax.Controllers
{
    public class HomeController : Controller
    {
        //[HttpPost]
        //public ActionResult Index(HttpPostedFileBase file)
        //{

        //    if (file.ContentLength > 0)
        //    {
        //        var fileName = Path.GetFileName(file.FileName);
        //        var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
        //        file.SaveAs(path);
        //    }

        //    return RedirectToAction("Index");
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public string AnalyzeText(SearchInput data)
        {
            LinguisticWeb ling = new LinguisticWeb("hi");

            var langTree = ling.Main(data.context);

            ContextLogic contxtLogic = new ContextLogic(langTree);

            String result = contxtLogic.getContext(data.focus);

            //var wordArray = langTree[0].getWord();

            return result;

        }
    }
}