using Plugins.Shared.Library.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Plugins.Shared.Library.Controls
{
    public class IconFont : ContentControl
    {
        static IconFont()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconFont), new FrameworkPropertyMetadata(typeof(IconFont)));
        }

        public Icon Icon
        {
            get => (Icon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(IconFont), new PropertyMetadata());

    }

    public enum Icon
    {
        [UnicodeValue("\ue84d")]
        Play,
        [UnicodeValue("\ue750")]
        Stop,
        [UnicodeValue("\ue64b")]
        Delete,
        [UnicodeValue("\ue63d")]
        Ok,
        [UnicodeValue("\ue605")]
        Record,
        [UnicodeValue("\ue8b9")]
        Schedule,
        [UnicodeValue("\ue646")]
        Flow,
        [UnicodeValue("\ue66e")]
        Settings,
        [UnicodeValue("\ue688")]
        Refresh,
        [UnicodeValue("\ue8b9")]
        Time,
        [UnicodeValue("\ue8fe")]
        Plus,
        [UnicodeValue("\ue550")]
        Edit
    }
}
