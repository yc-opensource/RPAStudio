using Plugins.Shared.Library.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPARobot.Navigation
{
    public class MenuItem
    {
        public MenuItem(string key, Icon icon, string text)
        {
            Key = key;
            Text = text;
            Icon = icon;
        }

        public Icon Icon { get; set; }
        public string Text { get; set; }
        public string Key { get; set; }
    }
}
