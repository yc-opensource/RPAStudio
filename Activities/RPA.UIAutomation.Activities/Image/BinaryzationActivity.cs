using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(BinaryzationDesigner))]
    public sealed class BinaryzationActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Binaryzation"; } }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("FileName")]
        [Browsable(true)]
        [Description("图像的完整路径以及名称。如果设置了此属性，则忽略输入项中Image属性。")]
        public InArgument<string> FileName { get; set; }

        [Category("输入")]
        [RequiredArgument]
        [OverloadGroup("image")]
        [DisplayName("Image")]
        [Browsable(true)]
        [Description("要进行处理的图像，仅支持Image变量。如果设置了此属性，则忽略输入项中FileName属性。")]
        public InArgument<System.Drawing.Image> image { get; set; }

        [Category("输出")]
        [DisplayName("Image")]
        [Browsable(true)]
        [Description("处理完成的图像。仅支持Image变量。")]
        public OutArgument<System.Drawing.Image> _Image { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/image.png";
            }
        }

        //二值化处理
        public static Bitmap Binary(Bitmap img)
        {
            int width = img.Width;
            int height = img.Height;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* start = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (start[0] != 255)
                        {
                            start[0] = start[1] = start[2] = 0;
                        }
                        start += 4;
                    }
                    start += bdata.Stride - width * 4;
                }
            }
            img.UnlockBits(bdata);
            return img;
        }   

        System.Drawing.Image img;
        System.Drawing.Image imgBinary;
        private string imgBinaried;
        protected override void Execute(CodeActivityContext context)
        {
            img = image.Get(context);
            string fileName = this.FileName.Get(context);
            try
            {
                if (img != null)
                {

                }
                else
                {
                    Bitmap bit = new Bitmap(fileName);
                    img = bit as System.Drawing.Image;
                }
                imgBinary = (System.Drawing.Image)img.Clone();
                imgBinary = Binary((Bitmap)imgBinary);
                Common com = new Common();
                imgBinaried = com.Image2Num((Bitmap)imgBinary);
                _Image.Set(context, imgBinary);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
