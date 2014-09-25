
using AvalonEdit.Sample.Helper;
using AvalonEdit.Sample.Model;
//using Horizon.Client.HostedModules.DevProductivity.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace AvalonEdit.Sample.Controls
{
    /// <summary>
    /// Interaction logic for WorkspaceView.xaml
    /// </summary>
    public partial class WorkspaceView : UserControl
    {
        public WorkspaceView()
        {
            InitializeComponent();
            XmlTreeView.XmlTree.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        string currentFileName;

        void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            string folderpath = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            op.Title = "Select an XML Document";

            bool? myResult;
            myResult = op.ShowDialog();
            if (myResult != null && myResult == true)
            {
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }
                string filePath = folderpath + System.IO.Path.GetFileName(op.FileName);
                if (System.IO.Path.GetExtension(op.FileName).Equals(".xml"))
                {
                    System.IO.File.Copy(op.FileName, "SampleConfig.xml", true);
                }
                else
                {
                    MessageBox.Show("Must import an XML document");
                }
            }
        }

        void saveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            XmlEditor.textEditor.Save(currentFileName);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (((TreeViewItem)((TreeView)sender).SelectedItem) != null)
            {
                var selectedName = ((TreeViewItem)((TreeView)sender).SelectedItem).Header.ToString();
                var hierarchy = XmlTreeView.getLocation(((TreeViewItem)((TreeView)sender).SelectedItem));
                hierarchy.Reverse();
                TextDocumentHelper.EnsureLineVisible(XmlEditor.textEditor, TextDocumentHelper.findSpecificText(XmlEditor.textEditor, hierarchy));
            }
        }

        private void TreeView_Click(object sender, RoutedEventArgs e)
        {
            List<XMLObject> parents = TextDocumentHelper.GetParentsFromCaretOffset(XmlEditor.textEditor);
            XmlTreeView.insertXml(XmlEditor.textEditor.Text, parents);
        }

    }
}
