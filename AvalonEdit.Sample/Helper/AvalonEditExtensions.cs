using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AvalonEdit.Sample.Helper
{
    public static class AvalonEditExtensions
    {
      
        public static IHighlightingDefinition AddCustomHighlighting(this TextEditor textEditor, Stream xshdStream)
        {
            if (xshdStream == null)
                throw new InvalidOperationException("Could not find embedded resource");

            IHighlightingDefinition customHighlighting;

            // Load our custom highlighting definition
            using (XmlReader reader = new XmlTextReader(xshdStream))
            {
                customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // And register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", null, customHighlighting);

            return customHighlighting;
        }

        public static IHighlightingDefinition AddCustomHighlighting(this TextEditor textEditor, Stream xshdStream, string[] extensions)
        {
            if (xshdStream == null)
                throw new InvalidOperationException("Could not find embedded resource");

            IHighlightingDefinition customHighlighting;

            // Load our custom highlighting definition
            using (XmlReader reader = new XmlTextReader(xshdStream))
            {
                customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            // And register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", extensions, customHighlighting);

            return customHighlighting;
        }

        public static string GetWordUnderMouse(this TextDocument document, TextViewPosition position)
        {
            string wordHovered = string.Empty;

            var line = position.Line;
            var column = position.Column;

            var offset = document.GetOffset(line, column);
            //Debug.WriteLine("offset "+ offset);
            if (offset >= document.TextLength)
                offset--;

            var textAtOffset = document.GetText(offset, 1);
            //Debug.WriteLine("textatoffset " + textAtOffset);

            // Get text backward of the mouse position, until the first space
            while (!string.IsNullOrWhiteSpace(textAtOffset))
            {
                wordHovered = textAtOffset + wordHovered;

                offset--;

                if (offset < 0)
                    break;

                textAtOffset = document.GetText(offset, 1);
            }

            // Get text forward the mouse position, until the first space
            offset = document.GetOffset(line, column);
            if (offset < document.TextLength - 1)
            {
                offset++;

                textAtOffset = document.GetText(offset, 1);

                while (!string.IsNullOrWhiteSpace(textAtOffset))
                {
                    wordHovered = wordHovered + textAtOffset;

                    offset++;

                    if (offset >= document.TextLength)
                        break;

                    textAtOffset = document.GetText(offset, 1);
                }
            }

            return wordHovered;
        }

        


        public static string GetWordBeforeEndTag(TextEditor textEditor)
        {
            var wordBefore = "";

            var caretPosition = textEditor.CaretOffset - 2;

            Debug.WriteLine("textEditor.CaretOffset: " + textEditor.CaretOffset);
            Debug.WriteLine("textEditor.Document.GetLocation(caretPosition): " + textEditor.Document.GetLocation(caretPosition));

            var lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(caretPosition));

            Debug.WriteLine("LinetOffset: " + lineOffset);

            string text = textEditor.Document.GetText(lineOffset, 1);

            if (!text.Equals(">"))
            {

                // Get text backward of the mouse position, until the first space
                while (!string.IsNullOrWhiteSpace(text) && !text.Equals(">"))
                {
                    wordBefore = text + wordBefore;

                    if (caretPosition == 0)
                        break;

                    if (text.Equals("/"))
                    {
                        wordBefore = "";
                        break;
                    }
                    lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(--caretPosition));

                    text = textEditor.Document.GetText(lineOffset, 1);
                }
            }

            return wordBefore;
        }

        public static void closeTag(this TextEditor textEditor, string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var temp = textEditor.CaretOffset;
                textEditor.Document.Insert(textEditor.CaretOffset, "</" + s + ">");
                //generateXML(textEditor);
                textEditor.TextArea.Caret.Offset = temp;
            }

        }

        public static void generateXML(this TextEditor textEditor, string file)
        {
            XElement xe = XElement.Load(file);            
            textEditor.Document.Insert(textEditor.CaretOffset, xe.ToString());
        }
    }
}


