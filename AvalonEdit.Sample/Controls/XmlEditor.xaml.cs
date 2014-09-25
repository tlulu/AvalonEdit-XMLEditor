using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Folding;
using AvalonEdit.Sample.Helper;
using AvalonEdit.Sample.Model;
using ICSharpCode.AvalonEdit;

namespace AvalonEdit.Sample.Controls
{
    public partial class XmlEditor : UserControl
    {
        private readonly TextMarkerService textMarkerService;
        private ToolTip toolTip;

        private const string StyleXML = "XML";
        private const string StyleSQL = "SQL";

        private bool _internalContentSync = false;
        private FoldingManager foldingManager;
        private XmlFoldingStrategy foldingStrategy;
        private bool _foldingEnabled { get; set; }
        private TextEditor temp;


        public XmlEditor()
        {
            InitializeComponent();

            textMarkerService = new TextMarkerService(textEditor);
            TextView textView = textEditor.TextArea.TextView;
            textView.BackgroundRenderers.Add(textMarkerService);
            textView.LineTransformers.Add(textMarkerService);
            textView.Services.AddService(typeof(TextMarkerService), textMarkerService);

            textView.MouseHover += MouseHover;
            textView.MouseHoverStopped += TextEditorMouseHoverStopped;
            textView.VisualLinesChanged += VisualLinesChanged;

            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            textEditor.MouseHover += MouseHover;
            textEditor.KeyDown += OnKeyDownHandler;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();

            foldingStrategy = new XmlFoldingStrategy();
            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();

            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }

            temp = new TextEditor();
            
        }

        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (checkValidXML())
            {
                temp = textEditor;
            }
            if (e.Text == " ")
            {
                if (checkValidXML())
                {
                    if (TextDocumentHelper.IsOffsetWithinElement(textEditor, textEditor.CaretOffset))
                    {
                        if (TextDocumentHelper.OffsetNotWithinQuotation(textEditor, textEditor.CaretOffset))
                        {
                            ActivateAttributeIntellisense();
                        }
                    }
                }

            }

            if (e.Text == "<")
            {
                ActivateElementIntellisense(temp);
            }


            if (e.Text == ">")
            {
                var previousWord = AvalonEditExtensions.GetWordBeforeEndTag(textEditor);
                Debug.WriteLine(previousWord);
                if (!previousWord.Equals("") && !previousWord.Equals("--"))
                {
                    previousWord = previousWord.Substring(1);
                    AvalonEditExtensions.closeTag(textEditor, previousWord);
                }
            }

        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.E))
            {
                Debug.WriteLine(textEditor.SelectionLength);
                if (textEditor.SelectionLength > 0)
                {
                    textEditor.Document.Insert(textEditor.SelectionStart, "<!--");
                    textEditor.Document.Insert(textEditor.SelectionStart + textEditor.SelectionLength, "-->");
                }
            }
        }

        private void MouseHover(object sender, MouseEventArgs e)
        {
            var pos = textEditor.TextArea.TextView.GetPosition(e.GetPosition(textEditor.TextArea.TextView) + textEditor.TextArea.TextView.ScrollOffset);
            bool inDocument = pos.HasValue;
            if (inDocument)
            {
                TextLocation logicalPosition = new TextLocation(pos.Value.Line, pos.Value.Column);
                int offset = textEditor.Document.GetOffset(logicalPosition);

                var markersAtOffset = textMarkerService.GetMarkersAtOffset(offset);
                TextMarkerService.TextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);

                if (markerWithToolTip != null)
                {
                    if (toolTip == null)
                    {
                        toolTip = new ToolTip();
                        toolTip.Closed += ToolTipClosed;
                        toolTip.PlacementTarget = this;
                        toolTip.Content = new TextBlock
                        {
                            Text = markerWithToolTip.ToolTip,
                            TextWrapping = TextWrapping.Wrap
                        };
                        toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        void ToolTipClosed(object sender, RoutedEventArgs e)
        {
            toolTip = null;
        }

        void TextEditorMouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        private void VisualLinesChanged(object sender, EventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
        }

        private bool checkValidXML()
        {
            try
            {
                var document = new XmlDocument { XmlResolver = null };
                document.LoadXml(textEditor.Document.Text);
                return true;
            }
            catch (XmlException ex)
            {
                return false;
            }
        }

        public void Validate()
        {
            textMarkerService.Clear();
            IServiceProvider sp = textEditor;
            var markerService = (TextMarkerService)sp.GetService(typeof(TextMarkerService));
            markerService.Clear();
            bool success = true;

            try
            {
                var document = new XmlDocument { XmlResolver = null };
                document.LoadXml(textEditor.Document.Text);
            }
            catch (XmlException ex)
            {
                DisplayValidationError(ex.Message, ex.LinePosition, ex.LineNumber);
                textEditor.ScrollToLine(ex.LineNumber);
                success = false;
            }
            if (success)
            {
                //Perform custom validation
            }

        }

        private void ActivateElementIntellisense(TextEditor textEditor)
        {
            List<XMLObject> parents = TextDocumentHelper.GetParentsFromCaretOffset(textEditor);
            List<string> hierarchy = TextDocumentHelper.GetElementParentsFromCaretOffset(textEditor, parents);
            List<string> names = TextDocumentHelper.GetElementsFromPath(hierarchy);
            if (names.Count != 0 && names != null)
            {
                MakeWindow(names);
            }
        }

        private void ActivateAttributeIntellisense()
        {
            List<XMLObject> parents = TextDocumentHelper.GetParentsFromCaretOffset(textEditor);
            List<string> hierarchy = TextDocumentHelper.GetElementParentsFromCaretOffset(textEditor, parents);
            var pos = TextDocumentHelper.getPositionOfStartBrace(textEditor, textEditor.CaretOffset);
            var ele = TextDocumentHelper.GetTagName(textEditor, pos);
            List<string> names = TextDocumentHelper.GetAttributesFromPath(hierarchy, ele);
            if (names.Count != 0 && names != null)
            {
                MakeWindow(names);
            }
        }


        private void MakeWindow(List<string> list)
        {
            completionWindow = new CompletionWindow(textEditor.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            foreach (var s in list)
            {
                data.Add(new MyCompletionData(s));
            }
            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                completionWindow = null;
            };
        }


        private void DisplayValidationError(string message, int linePosition, int lineNumber)
        {
            if (lineNumber >= 1 && lineNumber <= textEditor.Document.LineCount)
            {
                int offset = textEditor.Document.GetOffset(new TextLocation(lineNumber, linePosition));
                int length = textEditor.Document.GetLineByNumber(lineNumber).Length;
                textMarkerService.Create(offset, length, message);
            }
        }


        #region Folding
        //FoldingManager foldingManager;
        //AbstractFoldingStrategy foldingStrategy;

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
        }
        #endregion

        

    }

}

