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

namespace Syntax.Models
{
    //......................................................
    //Creating skeleton Langtree
    //private class LangTree
    //{
    //    private String tag;
    //    private String word;
    //    private List<LangTree> children;

    //    public LangTree();
      
    //}
    //.......................................................

    public class ContextLogic
    {
        List<LangTree> langTree;

        public ContextLogic(List<LangTree> langTree)
        {
            this.langTree = langTree;
        }
        //public static Boolean langTreeHas(LangTree inputTree, String focusWord)
        //{
        //    // Checking if the list of input words contains the focus word. 
        //    //return inputTree.getWord().Contains(focusWord, StringComparison.OrdinalIgnoreCase); 
        //    return 


        //}

        

        public String getContext(String focusWord)
        {
            var listOfLeaf =  langTree[0].getTree();
            String focusTag = "";
            foreach(var leaf in listOfLeaf)
            {
                if (leaf.word == focusWord)
                {
                    focusTag = leaf.tag;
                    break;
                }
            }
            if (focusTag == "")
            {
                return null;
            }
            if (focusTag.Equals("NN"))
            {
                foreach(var leaf in listOfLeaf)
                {
                    if (leaf.tag.Equals("VBD") && !leaf.word.Equals("was", StringComparison.OrdinalIgnoreCase) && !leaf.word.Equals("is", StringComparison.OrdinalIgnoreCase))
                    {
                        return leaf.word;
                    }
                    else if (leaf.tag.Equals("JJ"))
                        return leaf.word;
                   
                }
            }
            return null; //"There was no context for the focus word";
            
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
                string inputJson = @"{" +
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

    public class LangTree
    {
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
                        word = childWord.Substring(0, childWord.Length - 1);
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

        //public LangTree getNode(String focusWord)
        //{
        //    LangTree output = ;
        //    if (word.Equals(focusWord, StringComparison.OrdinalIgnoreCase))
        //        return this;
        //    foreach (var child in children)
        //        child.getNode(focusWord);
        //    return output;
        //}

        public List<LangTree> getTree()
        {
            List<LangTree> result = new List<LangTree>();
            if (children.Count == 0)
            {
                result.Add(this);
                return result;
            }
            foreach (var child in children)
            {
                result = result.Concat(child.getTree()).ToList();
            }
            return result;
        }
        public List<String> getWord()
        {
            List<String> result = new List<String>();
            if (children.Count == 0)
            {
                result.Add(word);
                return result;
            }
            foreach (var child in children)
            {
                result = result.Concat(child.getWord()).ToList();
            }
            return result;
        }

       
    }
}