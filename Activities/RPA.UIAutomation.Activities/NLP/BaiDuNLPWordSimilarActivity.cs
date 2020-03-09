using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.NLP
{
    /// <summary>
    /// 词义相似度
    /// </summary>
    [Designer(typeof(BaiDuNLPWordSimilarDesigner))]
    public sealed class BaiDuNLPWordSimilarActivity : CodeActivity
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
        [Description("输入词，最大64字节")]
        public InArgument<string> Word1 { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [Browsable(true)]
        [Description("输入词，最大64字节")]
        public InArgument<string> Word2 { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [Description("两个词的相似度结果。")]
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
            string word1 = Word1.Get(context);
            string word2 = Word2.Get(context);
            string apiKey = APIKey.Get(context);
            string seKey = SecretKey.Get(context);
            try
            {
                var client = new Baidu.Aip.Nlp.Nlp(apiKey, seKey);
                //修改超时时间   
                client.Timeout = 60000;
                //带参数调用词义相似度
                string result = client.WordSimEmbedding(word1, word2).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
