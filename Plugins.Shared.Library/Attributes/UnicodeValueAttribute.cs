using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Shared.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class UnicodeValueAttribute : Attribute
    {
        public string Unicode { get; private set; }
        public UnicodeValueAttribute(string unicode)
        {
            Unicode = unicode;
        }
    }
}
