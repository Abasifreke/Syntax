using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syntax.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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
        public ActionResult AnalyzeText(SearchInput data)
        {
            LinguisticWeb ling = new LinguisticWeb("hi");

            var langTree = ling.Main();

            return View();
        }
    }

    public class LinguisticWeb
    {
        string fileText { get; set; }

        public LinguisticWeb(string fileText)
        {
            this.fileText = fileText;
        }

        public List<LangTree> Main()
        {
            return RunAsync();
        }

        List<LangTree> RunAsync()
        {
            List<LangTree> sentenceList = new List<LangTree>();

            var uri = new Uri("https://api.projectoxford.ai/linguistics/v1.0/analyze");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", "365e63e76a5246c5afe5bee813c2a9ac");
            var id = Guid.NewGuid();

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string inputJson= @"{" +
                    "'language' : 'en'," +
                    "'analyzerIds' : ['22a6b758-420f-4745-8a3c-46835a67c0d2']," +
                    "'text' : 'There was a dog. It was happy. There was a dog. It went on a walk. Rufus went on a walk. It was a dog. It is happy.'" + "}";
                streamWriter.Write(inputJson);
                streamWriter.Flush();
                streamWriter.Close();
                //JsonConvert.ToString("}";
                //object to pass
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                dynamic jsonResult = JsonConvert.DeserializeObject(result);
                Array sentences = ((JProperty)((JObject)((JArray)jsonResult).Children().ToArray()[0]).Children().ToArray()[1]).Children().ToArray()[0].ToArray(); //((JProperty)((JObject)((JArray)jsonResult).ChildrenTokens[0]).ChildrenTokens[1]).ChildrenTokens[0];

                foreach (var sent in sentences)
                {
                    sentenceList.Add(new LangTree(((JValue)sent).ToString()));
                }
            }

            return sentenceList;
        }
    }

    public class LangTree {
        public string tag;
        public string word;
        public List<LangTree> children;

        public LangTree(string words)
        {
            children = new List<LangTree>();
            string childWord = "";
            int openBracket = 0;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == '(' && openBracket == 0)
                {
                    childWord = "";
                    openBracket++;
                }
                else if (words[i] == '(')
                {
                    childWord += words[i];
                    openBracket++;
                }
                else if (words[i] == ' ' && openBracket == 1 && words[i - 1] != ')')
                {
                    tag = childWord;
                    childWord = "";
                }
                else if (words[i] == ')' && openBracket == 2)
                {
                    childWord += words[i];
                    openBracket--;
                    if (childWord[0] != '(')
                    {
                        childWord = childWord.Substring(1, childWord.Length - 1);
                    }
                    children.Add(new LangTree(childWord));
                    childWord = "";
                }
                else if (words[i] == ')')
                {
                    childWord += words[i];
                    openBracket--;
                    if (openBracket == 0)
                    {
                        word = childWord.Substring(0, childWord.Length-1);
                    }
                }
                else
                {
                    childWord += words[i];                    
                }
                if (openBracket == 0)
                {
                    break;
                }
            }
        }
    }
}