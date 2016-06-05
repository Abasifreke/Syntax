using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Syntax.Models
{
    //......................................................
    //Creating skeleton Langtree
    private class LangTree
    {
        private String tag;
        private String word;
        private List<LangTree> children;

        public LangTree();
      
    }
    //.......................................................

    public class ContextLogic
    {
        
        public static Boolean langTreeHas(LangTree inputTree, String focusWord)
        {
            // Checking if the list of input words contains the focus word. 
            return inputTree.getWord().contains(focusWord, OrdinalIgnoreCase); 
               
        }

        //LangTree inputStringTree = LangTree();
        //public static String getContext(LangTree inputStringTree, String focusWord)
        //{
           
                   
        //}

       
    }
}