using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Plugins.Shared.Library.Controls
{
    /// <summary>
    /// 加载中
    /// </summary>
    public class Spin : ContentControl
    {
        public Spin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spin), new FrameworkPropertyMetadata(typeof(Spin)));
        }

        public static readonly DependencyProperty SpinningProperty =
            DependencyProperty.Register(nameof(Spinning), typeof(bool), typeof(Spin), new PropertyMetadata(true));
        public bool Spinning
        {
            get => (bool)GetValue(SpinningProperty);
            set => SetValue(SpinningProperty, value);
        }

        public static readonly DependencyProperty TipProperty =
            DependencyProperty.Register(nameof(Tip), typeof(string), typeof(Spin), new PropertyMetadata("正在加载中"));
        public string Tip
        {
            get => (string)GetValue(TipProperty);
            set => SetValue(TipProperty, value);
        }
    }
}
