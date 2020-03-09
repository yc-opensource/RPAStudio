using System.Activities;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace RPA.Core.Activities.StringActivity
{
    public sealed class IsMatchActivity: CodeActivity<bool>
    {
        public new string DisplayName
        {
            get
            {
                return "IsMatch";
            }
        }
      
        [Category("输入")]
        [RequiredArgument]
        [DisplayName("Input")]
        [Description("要搜索匹配项的字符串。")]
        public InArgument<string> Input{ get; set; }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("Pattern")]
        [Description("要匹配的正则表达式模式。")]
        public InArgument<string> Pattern { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("RegexOptions")]
        public RegexOptions RegexOption{ get; set; }

        public void IsMatch()
        {
            this.RegexOption = (RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        protected override bool Execute(CodeActivityContext context)
        {
            return Regex.IsMatch(this.Input.Get(context), this.Pattern.Get(context), this.RegexOption);
        }
    }
}
