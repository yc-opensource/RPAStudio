using System.Activities;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace RPA.Core.Activities.StringActivity
{
    public sealed class Replace: CodeActivity<string>
    {
        public new string DisplayName
        {
            get
            {
                return "Replace";
            }
        }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("Input")]
        [Description("要替换的字符串。")]
        public InArgument<string> Input
        {
            get;
            set;
        }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("Pattern")]
        [Description("要匹配的正则表达式模式。")]
        public InArgument<string> Pattern
        {
            get;
            set;
        }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("RegexOption")]
        public RegexOptions RegexOption
        {
            get;
            set;
        }
        public Replace()
        {
            this.RegexOption = (RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("Replacement")]
        [Description("替换字符串。")]
        public InArgument<string> Replacement
        {
            get;
            set;
        }

        protected override string Execute(CodeActivityContext context)
        {
            return Regex.Replace(this.Input.Get(context), this.Pattern.Get(context), this.Replacement.Get(context), this.RegexOption);
        }
    }
}
