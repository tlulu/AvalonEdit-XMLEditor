using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Diagnostics;
using System.Text.RegularExpressions;
using AvalonEdit.Sample.Model;

//using Horizon.Client.HostedModules.DevProductivity.Model;

namespace AvalonEdit.Sample.Controls
{
    /// <summary>
    /// Interaction logic for XmlTreeView.xaml
    /// </summary>
    public partial class XmlTreeView : UserControl
    {
        TreeViewItem root = null;
        public List<XMLObject> hierarchy { get; set; }

        public XmlTreeView()
        {
            InitializeComponent();
            root = new TreeViewItem();
            XmlTree.Items.Add(root);
            hierarchy = new List<XMLObject>();

        }

        public void insertXml(string xml, List<XMLObject> list)
        {
            XElement xe = null;
            root = new TreeViewItem();
            root.IsExpanded = true;
            XmlTree.Items.Clear();
            hierarchy = list;
            try
            {
                xe = XElement.Parse(xml);
            }
            catch (System.Xml.XmlException e) { }

            if (xe != null)
            {
                var tree = xe.DescendantsAndSelf();
                root.Header = tree.First().Name;
                makeTree(xe, root, 1);
                XmlTree.Items.Add(root);

            }
        }


        public void makeTree(XElement xe, TreeViewItem parent, int index)
        {
            if (xe.Elements() == null || parent.Header == null)
            {
                return;
            }
            foreach (var des in xe.Elements())
            {
                var newItem = new TreeViewItem();
                if (des.Attribute("Name") == null && des.Attribute("ID") == null)
                {
                    newItem.Header = des.Name;
                }
                else if (des.Attribute("Name") != null)
                {
                    newItem.Header = des.Attribute("Name").Value;
                }
                else if (des.Attribute("ID") != null)
                {
                    newItem.Header = des.Attribute("ID").Value;
                }

                if (index < hierarchy.Count)
                {
                    if (hierarchy[index].ID.Equals(newItem.Header.ToString()) || hierarchy[index].Name.Equals(newItem.Header.ToString()))   
                    {
                        newItem.IsExpanded = true;
                        index++;
                    }
                }
                parent.Items.Add(newItem);
                makeTree(des, newItem, index);
            }
        }

        public List<string> getLocation(TreeViewItem item)
        {
            List<string> hierarchy = new List<string>();
            var track = item;
            while (!track.Equals(root))
            {
                hierarchy.Add(track.Header.ToString());
                if ((TreeViewItem)track.Parent != null)
                {
                    track = (TreeViewItem)track.Parent;
                }
            }
            hierarchy.Add(track.Header.ToString());

            return hierarchy;
        }

        // determine which elements we consider the same
        private static bool AreEquivalent(XElement a, XElement b)
        {
            if (a.Name != b.Name) return false;
            if (!a.HasAttributes && !b.HasAttributes) return true;
            if (!a.HasAttributes || !b.HasAttributes) return false;
            if (a.Attributes().Count() != b.Attributes().Count()) return false;

            return a.Attributes().All(attA => b.Attributes(attA.Name)
                .Count(attB => attB.Value == attA.Value) != 0);
        }

    }
}


