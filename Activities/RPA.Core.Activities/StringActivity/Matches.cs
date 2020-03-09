using System.Collections.Generic;
using System.Activities;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;

namespace RPA.Core.Activities.StringActivity
{
    public sealed class Matches: CodeActivity<IEnumerable<Match>>
    {
        public new string DisplayName
        {
            get
            {
                return "Matches";
            }
        }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("Input")]
        [Description("要搜索匹配项的字符串。")]
        public InArgument<string> Input { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("Pattern")]
        [Description("要匹配的正则表达式模式。")]
        public InArgument<string> Pattern { get; set;}

        [Category("输入")]
        [RequiredArgument]
        [DisplayName("RegexOption")]
        public RegexOptions RegexOption { get; set; }

        public Matches()
        {
            this.RegexOption = (RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }      

        protected override IEnumerable<Match> Execute(CodeActivityContext context)
        {
            return Regex.Matches(this.Input.Get(context), this.Pattern.Get(context), this.RegexOption).Cast<Match>();
        }
    }
}
