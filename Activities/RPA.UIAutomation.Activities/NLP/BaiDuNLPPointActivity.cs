using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.NLP
{
    /// <summary>
    /// 评论观点抽取
    /// </summary>
    [Designer(typeof(BaiDuNLPPointDesigner))]
    public sealed class BaiDuNLPPointActivity : CodeActivity
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
        [Description("评论内容，最大10240字节。")]
        public InArgument<string> Text { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [Description("两个文本的相似度结果。")]
        public OutArgument<string> Result { get; set; }

        InArgument<Int32> _type = 4;
        [Category("选项")]
        [DisplayName("评论行业类型")]
        [Description("1:酒店;2:KTV;3:丽人;4:美食餐饮;5:旅游;6:健康;7:教育;8:商业;9:房产;10:汽车;11:生活;12:购物;13:3C，默认为4（餐饮美食）。")]
        [Browsable(true)]
        public InArgument<Int32> type
        {
            get { return _type; }
            set { _type = value; }
        }

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
            string text = Text.Get(context);
            string apiKey = APIKey.Get(context);
            string seKey = SecretKey.Get(context);
            try
            {
                var client = new Baidu.Aip.Nlp.Nlp(apiKey, seKey);
                //修改超时时间   
                client.Timeout = 60000;  
                //设置可选参数
                var options = new Dictionary<string, object>
                {
                    {"type",type}
                };
                //带参数调用评论观点抽取
                string result = client.CommentTag(text,options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
