using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(GrayDesigner))]
    public sealed class GrayActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "CAPTCHA"; } }

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

        bool _pingjun = true;
        [Category("灰度方式")]
        [DisplayName("平均值法")]
        [Browsable(true)]
        public bool Pingjun
        {
            get
            {
                return _pingjun;
            }
            set
            {
                _pingjun = value;
            }
        }
        bool _max;
        [Category("灰度方式")]
        [DisplayName("最大值法")]
        [Browsable(true)]
        public bool Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }
        bool _quanzhong;
        [Category("灰度方式")]
        [DisplayName("加权平均")]
        [Browsable(true)]
        public bool Quanzhong
        {
            get
            {
                return _quanzhong;
            }
            set
            {
                _quanzhong = value;
            }
        }
        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/image.png";
            }
        }
        /// <summary>
        /// 图片灰度化处理指针法
        /// </summary>
        /// <param name="img">待处理图片</param>
        /// <param name="type">1：最大值；2：平均值；3：加权平均；默认平均值</param>
        /// <returns>灰度处理后的图片</returns>
        public static System.Drawing.Image Gray(Bitmap img, int type = 2)
        {
            Func<int, int, int, int> getGrayValue;
            switch (type)
            {
                case 1:
                    getGrayValue = GetGrayValueByMax;
                    break;
                case 2:
                    getGrayValue = GetGrayValueByPingjunzhi;
                    break;
                default:
                    getGrayValue = GetGrayValueByQuanzhong;
                    break;
            }
            int height = img.Height;
            int width = img.Width;
            BitmapData bdata = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int v = getGrayValue(ptr[0], ptr[1], ptr[2]);
                        ptr[0] = ptr[1] = ptr[2] = (byte)v;
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            img.UnlockBits(bdata);
            return img;
        }
        //最大值法
        private static int GetGrayValueByMax(int r, int g, int b)
        {
            int max = r;
            max = max > g ? max : g;
            max = max > b ? max : b;
            return max;
        }
        //平均值法
        private static int GetGrayValueByPingjunzhi(int r, int g, int b)
        {
            return (r + g + b) / 3;
        }
        //加权平均
        private static int GetGrayValueByQuanzhong(int b, int g, int r)
        {
            return (int)(r * 0.3 + g * 0.59 + b * 0.11);
        }     
       
        private int graytype = 2;
        private string imgGreied;
        System.Drawing.Image img;
        protected override void Execute(CodeActivityContext context)
        {
            string fileName = this.FileName.Get(context);
            img = image.Get(context);
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
                System.Drawing.Image img_grey = (System.Drawing.Image)img.Clone();
                Common com = new Common();
                string imgOriginal=com.Image2Num((Bitmap)img_grey);
                
                graytype = 2;
                if (_max)
                {
                    graytype = 1;
                }
                if (_quanzhong)
                {
                    graytype = 3;
                }
                img_grey = Gray((Bitmap)img_grey, graytype);
                imgGreied =com.Image2Num((Bitmap)img_grey);           
                
                _Image.Set(context, img_grey);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "有一个错误产生", e.Message);
            }     
        }
    }
}
