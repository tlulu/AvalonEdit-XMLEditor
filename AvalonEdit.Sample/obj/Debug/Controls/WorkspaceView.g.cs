﻿#pragma checksum "..\..\..\Controls\WorkspaceView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C5B88B7735EF5991D3DDEEB7061A0927"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AvalonEdit.Sample.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace AvalonEdit.Sample.Controls {
    
    
    /// <summary>
    /// WorkspaceView
    /// </summary>
    public partial class WorkspaceView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\..\Controls\WorkspaceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal AvalonEdit.Sample.Controls.XmlEditor XmlEditor;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Controls\WorkspaceView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal AvalonEdit.Sample.Controls.XmlTreeView XmlTreeView;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AvalonEdit.Sample;component/controls/workspaceview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\WorkspaceView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 21 "..\..\..\Controls\WorkspaceView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.openFileClick);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 24 "..\..\..\Controls\WorkspaceView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.saveFileClick);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 30 "..\..\..\Controls\WorkspaceView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TreeView_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.XmlEditor = ((AvalonEdit.Sample.Controls.XmlEditor)(target));
            return;
            case 5:
            this.XmlTreeView = ((AvalonEdit.Sample.Controls.XmlTreeView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

