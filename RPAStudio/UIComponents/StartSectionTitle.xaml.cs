using System;
using System.Collections.Generic;
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

namespace RPAStudio.UserControls
{
    /// <summary>
    /// StartSectionTitle.xaml 的交互逻辑
    /// </summary>
    public partial class StartSectionTitle : UserControl
    {
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(StartSectionTitle), new PropertyMetadata("Title"));

        public StartSectionTitle()
        {
            InitializeComponent();
        }
    }
}
