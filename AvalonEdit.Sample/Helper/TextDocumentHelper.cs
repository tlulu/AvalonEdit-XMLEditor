using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using AvalonEdit.Sample.Model;
using System.Windows;

namespace AvalonEdit.Sample.Helper
{
    public class TextDocumentHelper
    {
        public static int getLineposition(TextEditor textEditor, int index)
        {
            return textEditor.Document.GetLocation(index).Column;

        }

        public static int getLineNumber(TextEditor textEditor, int index)
        {
            return textEditor.Document.GetLocation(index).Line;

        }

        public static int findText(TextEditor textEditor, string word, int startIndex)
        {
            return textEditor.Text.IndexOf(word, startIndex, StringComparison.InvariantCulture);
        }

        public static void EnsureLineVisible(TextEditor textEditor, int foundIndex)
        {
            var foundLine = textEditor.TextArea.Document.GetLineByOffset(foundIndex);
            if (foundLine != null)
            {
                var foundLineNo = foundLine.LineNumber;
                textEditor.TextArea.TextView.EnsureVisualLines();
                var visualLine = textEditor.TextArea.TextView.GetVisualLine(foundLineNo);
                if (visualLine == null)
                {
                    textEditor.ScrollToLine(textEditor.TextArea.Document.GetLineByOffset(foundIndex).LineNumber);
                }
            }
        }

        //public static int findSpecificText(TextEditor textEditor, List<string> list)
        //{
        //    //List<XMLObject> XmlList = GetElementPositionList(textEditor);
        //    string TextToFind;
        //    int startIndex = 0;
        //    //int start=0;
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        TextToFind = list[i];

        //        startIndex = findText(textEditor, TextToFind, startIndex);
        //    }

        //    if (list.Count - 1 >= 0)   //highlighting the text
        //    {
        //        textEditor.Select(startIndex, list[list.Count - 1].Length);
        //    }
        //    return startIndex;

        //}

        public static int findSpecificText(TextEditor textEditor, List<string> list)
        {
            List<XMLObject> XmlList = GetElementPositionList(textEditor);
            string TextToFind;
            int start = 0;
            int found = 0;
            for (int i = 0; i < list.Count; i++)
            {
                TextToFind = list[i];

                for (int j = start; j < XmlList.Count; j++)
                {
                    if (TextToFind.Equals(XmlList[j].Name) || TextToFind.Equals(XmlList[j].ID))
                    {
                        start = j + 1;
                        found = XmlList[j].Start;
                        break;
                    }
                }
            }

            if (list.Count - 1 >= 0)   //highlighting the text
            {
                DocumentLine line = textEditor.Document.GetLineByOffset(found);
                textEditor.Select(line.Offset, line.Length - 1);
            }

            return found;

        }

        public static bool OffsetNotWithinQuotation(TextEditor textEditor, int start)
        {
            int endOffset = start;
            string text;
            var doc = textEditor.Text;
            var length = doc.Length;
            int left = 0;
            int right = 0;

            while (endOffset < length)
            {
                text = doc.Substring(endOffset, 1);
                if (text.Equals("\""))
                {
                    right++;
                }
                else if (text.Equals(">"))
                {
                    break;
                }
                endOffset++;
            }
            endOffset = start;
            while (endOffset >= 1)
            {
                endOffset--;
                text = doc.Substring(endOffset, 1);
                if (text.Equals("<"))
                {
                    break;
                }
                else if (text.Equals("\""))
                {
                    left++;
                }
            }

            return left % 2 == 0 && right % 2 == 0;
        }

        public static bool IsOffsetWithinElement(TextEditor textEditor, int start)
        {
            int endOffset = start;
            string text;
            var doc = textEditor.Text;
            var length = doc.Length;

            while (endOffset < length)
            {
                text = doc.Substring(endOffset, 1);
                if (text.Equals("<"))
                {
                    return false;
                }
                else if (text.Equals(">"))
                {
                    return true;
                }
                endOffset++;
            }
            endOffset = start;
            while (endOffset >= 1)
            {
                endOffset--;
                text = doc.Substring(endOffset, 1);
                if (text.Equals("/"))
                {
                    return false;
                }
                else if (text.Equals("<"))
                {
                    return true;
                }
            }

            return true;
        }

        public static int getPositionOfStartBrace(TextEditor textEditor, int start)
        {
            int endOffset = start;
            string text;
            var doc = textEditor.Text;
            var length = doc.Length;

            while (endOffset >= 1)
            {
                endOffset--;
                text = doc.Substring(endOffset, 1);
                if (text.Equals("<"))
                {
                    break;
                }
            }
            return endOffset;
        }

        public static string GetTagName(TextEditor textEditor, int start)   //start is the index of "<"
        {
            var doc = textEditor.Text;
            var length = doc.Length;
            int endOffset = start;
            string text = textEditor.Document.GetText(start, 1);
            string word = "";

            while (!string.IsNullOrWhiteSpace(text) && !text.Equals(">") && endOffset < length)
            {
                endOffset++;
                text = doc.Substring(endOffset, 1);
                word += text;
            }
            return word.Remove(word.Length - 1);
        }


        public static string GetTagID(TextEditor textEditor, int start)   //start is the index of "<"
        {
            int endOffset = start;
            XElement xe = null;
            string text = textEditor.Document.GetText(start, 1);
            string word = "<";
            string id = "";
            bool one = false;
            var doc = textEditor.Text;
            var length = doc.Length;

            while (!text.Equals(">") && endOffset < length)
            {
                endOffset++;
                text = doc.Substring(endOffset, 1);
                if (endOffset + 1 < length && doc.Substring(endOffset, 2).Equals("/>"))
                {
                    one = true;
                    break;
                }
                word += text;
            }
            if (one)
            {
                word = word + "/>";
            }
            else
            {
                word = word + "</" + GetTagName(textEditor, start) + ">";
            }
            try
            {
                xe = XElement.Parse(word);
            }
            catch (XmlException ex)
            {
                var s = word;
                //throw new XmlException(ex.Message);
            }

            if (xe != null)
            {
                if (xe.Attribute("Name") == null && xe.Attribute("ID") == null)
                {
                    id = xe.Name.ToString();
                }
                else if (xe.Attribute("Name") != null)
                {
                    id = xe.Attribute("Name").Value;
                }
                else if (xe.Attribute("ID") != null)
                {
                    id = xe.Attribute("ID").Value;
                }
            }

            return id;
        }

        public static List<XMLObject> GetElementPositionList(TextEditor textEditor)    //returns a list of the position, name, id of each xml element
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<XMLObject> XmlList = new List<XMLObject>();
            Stack<XMLObject> XmlStack = new Stack<XMLObject>();
            bool comment = false;
            //bool outside = false;
            var caretOffset = textEditor.CaretOffset;
            var doc = textEditor.Text;
            var length = doc.Length;
            //var length = textEditor.Document.TextLength;

            for (int i = 0; i < length; i++)
            {
                if (i + 1 < length && (doc.Substring(i, 2).Equals("/>") || doc.Substring(i, 2).Equals("</")))
                {
                    if (XmlStack.Count != 0 && comment == false)
                    {
                        var obj = XmlStack.Peek();
                        obj.End = i;
                        XmlList.Add(obj);
                        XmlStack.Pop();

                    }

                }
                else if (i + 3 < length && doc.Substring(i, 4).Equals("<!--"))
                {
                    comment = true;
                }
                else if (i + 2 < length && doc.Substring(i, 3).Equals("-->"))
                {
                    comment = false;
                }
                else if (doc.Substring(i, 1).Equals("<") && (i + 1 < length && !doc.Substring(i, 2).Equals("<!")))
                {
                    if (comment == false)
                    {
                        var obj = new XMLObject();
                        obj.Name = GetTagName(textEditor, i);
                        obj.ID = GetTagID(textEditor, i);      //takes 0.4 seconds    //texteditor.document.getText() takes too long
                        obj.Start = findText(textEditor, ">", i);
                        XmlStack.Push(obj);
                    }

                }
            }

            XmlList = XmlList.OrderBy(x => x.Start).ToList();

            sw.Stop();
            string ExecutionTimeTaken = string.Format("Minutes :{0}  Seconds :{1}  Mili seconds :{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.TotalMilliseconds);
            Debug.WriteLine(ExecutionTimeTaken);

            return XmlList;
        }

        public static List<XMLObject> GetParentsFromCaretOffset(TextEditor textEditor)    //returns only the elements that surround the cursor
        {
            List<XMLObject> XmlList = GetElementPositionList(textEditor);
            var currentOffset = textEditor.CaretOffset;
            List<XMLObject> hierarchy = new List<XMLObject>();
            for (int i = 0; i < XmlList.Count; i++)
            {
                if (currentOffset > XmlList[i].Start && currentOffset < XmlList[i].End)
                {
                    hierarchy.Add(XmlList[i]);
                }
            }

            return hierarchy;
        }

        public static List<string> GetElementParentsFromCaretOffset(TextEditor textEditor, List<XMLObject> XmlList)   //string list version of getParentsFromCaretOffset
        {
            List<string> hierarchy = new List<string>();
            for (int i = 0; i < XmlList.Count; i++)
            {
                hierarchy.Add(XmlList.ElementAt(i).Name);
            }
            return hierarchy;
        }



        public static List<string> GetElementsFromPath(List<string> hierarchy)   //For Element intellisense. Gets list of elements available based on the elements that you're trapped within.
        {
            List<string> names = new List<string>();
            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create("SampleConfig.xml");

            }
            catch (Exception ex)
            {
                MessageBox.Show("No xml document exists in the folder. Add SampleConfig.xml to the folder where Horizon is installed");
            }

            if (xmlReader != null)
            {
                XDocument myXDocument = XDocument.Load(xmlReader);
                string path = "";
                for (int i = 0; i < hierarchy.Count; i++)    /////check index
                {
                    path = path + "/" + hierarchy[i];
                }
                var namespaceManager = new XmlNamespaceManager(xmlReader.NameTable); // We now have a namespace manager that knows of the namespaces used in your document.
                namespaceManager.AddNamespace("prefix", "http://www.MyNamespace.ca/MyPath"); // We add an explicit prefix mapping for our query.

                //if (hierarchy.Count != 0)
                // {
                path = "/" + myXDocument.Root.Name + path;
                var result = myXDocument.XPathSelectElements(path, namespaceManager); // We use that prefix against the elements in the query.

                if (result != null)
                {
                    foreach (var res in result)
                    {
                        foreach (var ele in res.Elements())
                        {
                            names.Add(ele.Name.ToString());
                        }
                        names = names.Distinct().ToList();
                    }
                }
            }

            for (int i = 0; i < hierarchy.Count; i++)
            {
                Debug.Write(hierarchy[i] + " ");
            }
            Debug.WriteLine("");

            return names;
        }

        public static List<string> GetAttributesFromPath(List<string> hierarchy, string element)
        {
            List<string> names = new List<string>();
            XmlReader xmlReader = null;
            try
            {
                xmlReader = XmlReader.Create("SampleConfig.xml");

            }
            catch (Exception ex)
            {
                MessageBox.Show("No xml document exists in the folder");
            }

            if (xmlReader != null)
            {
                XDocument myXDocument = XDocument.Load(xmlReader);
                string path = "";
                for (int i = 1; i < hierarchy.Count; i++)    /////check index
                {
                    path = path + "/" + hierarchy[i];
                }
                var namespaceManager = new XmlNamespaceManager(xmlReader.NameTable); // We now have a namespace manager that knows of the namespaces used in your document.
                namespaceManager.AddNamespace("prefix", "http://www.MyNamespace.ca/MyPath"); // We add an explicit prefix mapping for our query.

                if (hierarchy.Count != 0)
                {
                    path = "/" + myXDocument.Root.Name + path;
                    var result = myXDocument.XPathSelectElements(path, namespaceManager); // We use that prefix against the elements in the query.

                    if (result != null)
                    {
                        foreach (var res in result.Elements())
                        {
                            if (res.Name.ToString().Equals(element))
                            {
                                foreach (var atr in res.Attributes())
                                {
                                    names.Add(atr.Name.ToString());
                                }
                                names = names.Distinct().ToList();
                            }
                        }
                    }
                }
            }


            return names;
        }

    }
}
