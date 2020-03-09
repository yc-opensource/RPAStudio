using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.NLP
{
    /// <summary>
    /// 新闻摘要
    /// </summary>
    [Designer(typeof(BaiDuNLPNewsDesigner))]
    public sealed class BaiDuNLPNewsActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "BaiDuNLP"; } }

        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [Description("您的APIKey。")]
        public InArgument<string> APIKey { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [Description("您的SecretKey。")]
        public InArgument<string> SecretKey { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [Description("字符串。")]
        public InArgument<string> Content { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [Description("标题。")]
        public InArgument<string> Title { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [Description("摘要结果的最大长度。")]
        public InArgument<Int32> MaxLen { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [Description("输出指定长度的新闻摘要。")]
        public OutArgument<string> Result { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/NLP/nlp.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string content = Content.Get(context);
            string apiKey = APIKey.Get(context);
            string seKey = SecretKey.Get(context);
            string title = Title.Get(context);
            int maxlen = MaxLen.Get(context);
            try
            {
                var client = new Baidu.Aip.Nlp.Nlp(apiKey, seKey);
                // 修改超时时间
                client.Timeout = 60000;  
                //设置可选参数
                var options = new Dictionary<string, object>
                {
                    {"title", title}
                };
                //带参数调用新闻摘要接口
                string result = client.NewsSummary(content, maxlen, options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
