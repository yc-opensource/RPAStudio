using Baidu.Aip.Ocr;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace RPA.UIAutomation.Activities.OCR
{
    /// <summary>
    /// 百度云OCR网络图片文字识别
    /// </summary>
    [Designer(typeof(BaiDuOCRWebImageDesigner))]
    public sealed class BaiDuOCRWebImageActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "BaiDuOCR"; } }

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
        [OverloadGroup("FileName")]
        [Browsable(true)]
        [Description("图片的完整路径以及名称。如果设置了此属性，则忽略输入项中Image属性。")]
        public InArgument<string> FileName { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("image")]
        [DisplayName("Image")]
        [Browsable(true)]
        [Description("要进行文本识别的图像，仅支持Image变量。如果设置了此属性，则忽略输入项中FileName属性。")]
        public InArgument<System.Drawing.Image> image { get; set; }

        [Category("输出")]
        [Browsable(true)]
        [Description("图片识别结果。")]
        public OutArgument<string> Result { get; set; }

        bool Detect_direction = false;
        [Category("选项")]
        [DisplayName("检测图像朝向")]
        [Browsable(true)]
        [Description("是否检测图像朝向，默认不检测。朝向是指输入图像是正常方向还是逆时针旋转90/180/270度。")]
        public bool detect_direction
        {
            get { return Detect_direction; }
            set { Detect_direction = value; }
        }

        bool Detect_language = false;
        [Category("选项")]
        [DisplayName("检测语言")]
        [Browsable(true)]
        [Description("是否检测语言，默认不检测。当前支持（中文、英语、日语、韩语）")]
        public bool detect_language
        {
            get { return Detect_language; }
            set { Detect_language = value; }
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/OCR/OCR.png";
            }
        }

        //将图片转换成字节数组
        public byte[] SaveImage(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
            return imgBytesIn;
        }

        //将Image变量转换成字节数组
        public byte[] ConvertImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        System.Drawing.Image img;
        byte[] by;
        protected override void Execute(CodeActivityContext context)
        {
            string path = FileName.Get(context);
            string API_KEY = APIKey.Get(context);
            string SECRET_KEY = SecretKey.Get(context);
            img = image.Get(context);
            try
            {
                if (path != null)
                {
                    by = SaveImage(path);
                }
                else
                {
                    by = ConvertImageToByte(img);
                }
                var client = new Ocr(API_KEY, SECRET_KEY);
                //修改超时时间   
                client.Timeout = 60000; 
                //参数设置               
                var options = new Dictionary<string, object>
                {
                    {"detect_direction",detect_direction.ToString().ToLower()},
                    {"detect_language", detect_language.ToString().ToLower()}
                };
                //带参数调用网络图片文字识别
                string result = client.WebImage(by, options).ToString();
                Result.Set(context, result);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
