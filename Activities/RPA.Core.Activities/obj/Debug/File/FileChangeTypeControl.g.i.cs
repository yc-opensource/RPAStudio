﻿#pragma checksum "..\..\..\File\FileChangeTypeControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9536E113EC6789EC3AB2FC009C456F0B837C6189EF5DCA4B73414D4E788EFEC6"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using RPA.Core.Activities.FileActivity;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace RPA.Core.Activities.FileActivity {
    
    
    /// <summary>
    /// FileChangeTypeControl
    /// </summary>
    public partial class FileChangeTypeControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\File\FileChangeTypeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbCreated;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\File\FileChangeTypeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbDeleted;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\File\FileChangeTypeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbChanged;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\File\FileChangeTypeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbRenamed;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\File\FileChangeTypeControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbAll;
        
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
            System.Uri resourceLocater = new System.Uri("/RPA.Core.Activities;component/file/filechangetypecontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\File\FileChangeTypeControl.xaml"
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
            
            #line 10 "..\..\..\File\FileChangeTypeControl.xaml"
            ((System.Windows.Controls.ComboBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.ComboBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cbCreated = ((System.Windows.Controls.CheckBox)(target));
            
            #line 18 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbCreated.Checked += new System.Windows.RoutedEventHandler(this.Created_Checked);
            
            #line default
            #line hidden
            
            #line 18 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbCreated.Unchecked += new System.Windows.RoutedEventHandler(this.Created_Checked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cbDeleted = ((System.Windows.Controls.CheckBox)(target));
            
            #line 19 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbDeleted.Checked += new System.Windows.RoutedEventHandler(this.Deleted_Checked);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbDeleted.Unchecked += new System.Windows.RoutedEventHandler(this.Deleted_Checked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.cbChanged = ((System.Windows.Controls.CheckBox)(target));
            
            #line 20 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbChanged.Checked += new System.Windows.RoutedEventHandler(this.Changed_Checked);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbChanged.Unchecked += new System.Windows.RoutedEventHandler(this.Changed_Checked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.cbRenamed = ((System.Windows.Controls.CheckBox)(target));
            
            #line 21 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbRenamed.Checked += new System.Windows.RoutedEventHandler(this.Renamed_Checked);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbRenamed.Unchecked += new System.Windows.RoutedEventHandler(this.Renamed_Checked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.cbAll = ((System.Windows.Controls.CheckBox)(target));
            
            #line 22 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbAll.Checked += new System.Windows.RoutedEventHandler(this.All_Checked);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\File\FileChangeTypeControl.xaml"
            this.cbAll.Unchecked += new System.Windows.RoutedEventHandler(this.All_Checked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

