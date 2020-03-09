using Plugins.Shared.Library;
using System;
using System.Activities;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.Image
{
    [Designer(typeof(SaveImageDesigner))]
    public sealed class SaveImageActivity : CodeActivity
    {
        public new string DisplayName;
        [Browsable(false)]
        public string _DisplayName { get { return "Save Image"; } }      

        [Category("公共")]
        [DisplayName("错误执行")]
        [Description("指定即使当前活动失败，也要继续执行其余的活动。只支持布尔值(True,False)。")]
        public InArgument<bool> ContinueOnError { get; set; }

        [RequiredArgument]
        [Category("输入")]
        [Description("保存图像的完整文件路径及其名称。")]
        public InArgument<string> FileName { get; set; }

        [RequiredArgument]
        [Category("输入")]
        [DisplayName("Image")]
        [Browsable(true)]
        [Description("要保存的图像。仅支持Image变量。")]
        public InArgument<System.Drawing.Image> Image { get; set; }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Image/SaveImage.png";
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                string fileName = FileName.Get(context);
                System.Drawing.Image image = Image.Get(context);
                image.Save(fileName);
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "保存图片失败", e.Message);
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
