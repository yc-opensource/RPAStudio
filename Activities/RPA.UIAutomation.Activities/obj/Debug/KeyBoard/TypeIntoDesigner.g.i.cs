﻿#pragma checksum "..\..\..\KeyBoard\TypeIntoDesigner.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F5C14C7ACFF1999AE150DD7AB98923B4C67BABB8163FD63ED6A5F5FFB6902991"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Plugins.Shared.Library.Controls;
using RPA.UIAutomation.Activities.Keyboard;
using System;
using System.Activities.Presentation;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.View;
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


namespace RPA.UIAutomation.Activities.Keyboard {
    
    
    /// <summary>
    /// TypeIntoDesigner
    /// </summary>
    public partial class TypeIntoDesigner : System.Activities.Presentation.ActivityDesigner, System.Windows.Markup.IComponentConnector {
        
        
        #line 35 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock navigateTextBlock;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Hyperlink Hyperlink;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image navigateImage;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button navigateButton;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContextMenu contextMenu;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Activities.Presentation.View.ExpressionTextBox expressTextBox;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBox;
        
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
            System.Uri resourceLocater = new System.Uri("/RPA.UIAutomation.Activities;component/keyboard/typeintodesigner.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
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
            
            #line 10 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
            ((RPA.UIAutomation.Activities.Keyboard.TypeIntoDesigner)(target)).Loaded += new System.Windows.RoutedEventHandler(this.ActivityDesigner_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.navigateTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.Hyperlink = ((System.Windows.Documents.Hyperlink)(target));
            
            #line 36 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
            this.Hyperlink.Click += new System.Windows.RoutedEventHandler(this.HyperlinkClick);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 38 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
            ((System.Windows.Controls.Button)(target)).MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.Button_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.navigateImage = ((System.Windows.Controls.Image)(target));
            return;
            case 6:
            this.grid = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.navigateButton = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
            this.navigateButton.Click += new System.Windows.RoutedEventHandler(this.NavigateButtonClick);
            
            #line default
            #line hidden
            
            #line 42 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
            this.navigateButton.Initialized += new System.EventHandler(this.NavigateButtonInitialized);
            
            #line default
            #line hidden
            return;
            case 8:
            this.contextMenu = ((System.Windows.Controls.ContextMenu)(target));
            return;
            case 9:
            
            #line 48 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.meauItemClickOne);
            
            #line default
            #line hidden
            return;
            case 10:
            this.expressTextBox = ((System.Activities.Presentation.View.ExpressionTextBox)(target));
            return;
            case 11:
            this.comboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 72 "..\..\..\KeyBoard\TypeIntoDesigner.xaml"
            this.comboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.comboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

