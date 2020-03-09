using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace ImageOperaActivity
{
    [Designer(typeof(ImageConvertBase64Designer))]
    public sealed class ImageConvertBase64Activity : CodeActivity
    {
        public new string DisplayName
        {
            get
            {
                return "Image Convert";
            }
        }

        [RequiredArgument]
        [Category("输入")]
        [Description("要读取的文件的路径。")]
        public InArgument<string> FileName { get; set; }

        [Category("输出")]
        [Description("输出Base64。")]
        public OutArgument<string> Content { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/ImageConvertBase64.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            string Imagefilename = FileName.Get(context);
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                string strbaser64 = Convert.ToBase64String(arr);
                Content.Set(context, strbaser64);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
