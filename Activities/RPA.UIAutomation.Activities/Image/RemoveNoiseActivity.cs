using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(RemoveNoiseDesigner))]
    public sealed class RemoveNoiseActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "RemoveBackground"; } }

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

        InArgument<Int32> _Noise = 1;
        [Category("输入")]
        [DisplayName("噪点阈值")]
        [Description("合适的噪点阈值可达到更好的去噪点效果。")]
        [Browsable(true)]
        public InArgument<Int32> Noise
        {
            get
            {
                return _Noise;
            }
            set
            {
                _Noise = value;
            }
        }

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

        public static System.Drawing.Image RemoveNoise(Bitmap img, int maxAroundPoints = 1)
        {
            int width = img.Width;
            int height = img.Height;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb);
            #region 指针法
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (i == 0 || i == height - 1 || j == 0 || j == width - 1) //边界点，直接当作噪点去除掉
                        {
                            ptr[0] = ptr[1] = ptr[2] = 255;
                        }
                        else
                        {
                            int aroundPoint = 0;
                            if (ptr[0] != 255) //看标记，不是背景点
                            {
                                //判断其周围8个方向与自己相连接的有几个点
                                if ((ptr - 4)[0] != 255) aroundPoint++; //左边
                                if ((ptr + 4)[0] != 255) aroundPoint++; //右边
                                if ((ptr - width * 4)[0] != 255) aroundPoint++; //正上方
                                if ((ptr - width * 4 + 4)[0] != 255) aroundPoint++; //右上角
                                if ((ptr - width * 4 - 4)[0] != 255) aroundPoint++; //左上角
                                if ((ptr + width * 4)[0] != 255) aroundPoint++; //正下方
                                if ((ptr + width * 4 + 4)[0] != 255) aroundPoint++; //右下方
                                if ((ptr + width * 4 - 4)[0] != 255) aroundPoint++; //左下方
                            }
                            if (aroundPoint < maxAroundPoints)//目标点是噪点
                            {
                                ptr[0] = ptr[1] = ptr[2] = 255; //去噪点
                            }
                        }
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            img.UnlockBits(bdata);
            #endregion
            return img;
        }

        System.Drawing.Image img;
        private string imgNoised;
        //private static string imgurl;
        protected override void Execute(CodeActivityContext context)
        {
            img = image.Get(context);
            string fileName = this.FileName.Get(context);
            Int32 noise = Noise.Get(context);
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
                System.Drawing.Image img_noise = RemoveNoise((Bitmap)img.Clone(), noise);
                Common com = new Common();
                imgNoised = com.Image2Num((Bitmap)img_noise);
              //  WriteToFile(imgNoised, "experiment\\" + Path.GetFileNameWithoutExtension(imgurl) + "_noised.txt");
                _Image.Set(context, img_noise);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }
        }
    }
}
