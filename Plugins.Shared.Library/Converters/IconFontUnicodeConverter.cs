using Plugins.Shared.Library.Attributes;
using Plugins.Shared.Library.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Plugins.Shared.Library.Converters
{
    public class IconFontUnicodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var icon = (Icon)value;
            var fieldInfo = icon.GetType().GetField(icon.ToString());
            var attributes = fieldInfo.GetCustomAttributes(false);
            var attribute = attributes.FirstOrDefault(a => a is UnicodeValueAttribute) as UnicodeValueAttribute;
            return attribute?.Unicode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
