using System;
using System.Activities;
using System.ComponentModel;
using System.Drawing;
using Plugins.Shared.Library;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(LoadImageDesigner))]
    public sealed class LoadImageActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Load Image"; } }

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("输入")]
        [Description("要载入的图像的完整路径。")]
        public InArgument<string> FileName { get; set; }

        [RequiredArgument]
        [Category("输出")]
        [DisplayName("Image")]
        [Browsable(true)]      
        [Description("要载入的图像。仅支持图像变量。")]
        public OutArgument<System.Drawing.Image> Image { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/LoadImage.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string fileName = this.FileName.Get(context);
                Bitmap bit = new Bitmap(fileName);
                System.Drawing.Image img = bit as System.Drawing.Image;
                Image.Set(context, img);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "载入图片失败", e.Message);
                if (ContinueOnError.Get(context))
                {

                }
                else
                {
                    throw e;
                }
            }
            
        }        
    }
    
}
