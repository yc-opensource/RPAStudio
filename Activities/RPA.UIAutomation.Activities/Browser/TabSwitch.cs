using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(GoBackDesigner))]
    public sealed class TabSwitch : AsyncCodeActivity
    {
        static TabSwitch()
        {
        }
        private string classID = Guid.NewGuid().ToString("N");

        [Category("选项")]
        [DisplayName("标签页")]
        [Description("标签页页面顺序")]
        [RequiredArgument]
        [OverloadGroup("TabSeq")]
        public InArgument<Int32> TabSeq { get; set; }

        [Category("选项")]
        [DisplayName("关键字")]
        [RequiredArgument]
        [OverloadGroup("TabKey")]
        [Description("标签页标签/地址关键字")]
        public InArgument<string> TabKey { get; set; }


        [Category("输入")]
        [Browsable(true)]
        [DisplayName("浏览器Browser")]
        [Description("要关闭的浏览器页面。该字段仅支持Browser变量")]
        public InArgument<Browser> currBrowser { get; set; }


        [Browsable(false)]
        public string guid { get { return classID; } }
        [Browsable(false)]
        public string SourceImgPath { get; set; }
        [Browsable(false)]
        public string ClassName { get { return "TabSwitch"; } }


        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/TabSwitch.png";
            }
        }

        private System.Windows.Visibility visi = System.Windows.Visibility.Hidden;
        [Browsable(false)]
        public System.Windows.Visibility visibility
        {
            get
            {
                return visi;
            }
            set
            {
                visi = value;
            }
        }

        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Browser thisBrowser = currBrowser.Get(context);
            try
            {
                //先走逻辑 其次走流程
                if (thisBrowser == null)
                {
                    PropertyDescriptor property = context.DataContext.GetProperties()[OpenBrowser.OpenBrowsersPropertyTag];
                    if (property == null)
                        property = context.DataContext.GetProperties()[AttachBrowser.OpenBrowsersPropertyTag];
                    if (property == null)
                    {
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "活动流程传递的浏览器变量为空，请检查！");
                        m_Delegate = new runDelegate(Run);
                        return m_Delegate.BeginInvoke(callback, state);
                    }
                    Browser getBrowser = property.GetValue(context.DataContext) as Browser;
                    IWebDriver driver = getBrowser.getICFBrowser();
                    TabSwitchFunc(context, driver);
                }
                else
                {
                    if (thisBrowser.getICFBrowser() != null)
                    {
                        IWebDriver driver = thisBrowser.getICFBrowser() as ChromeDriver;
                        TabSwitchFunc(context, driver);
                    }
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "标签页切换失败", e);
                throw e;
            }


            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }

        void TabSwitchFunc(AsyncCodeActivityContext context, IWebDriver driver)
        {
            if (TabSeq.Expression != null)
            {
                Int32 tabSeq = TabSeq.Get(context);
                if (tabSeq >= driver.WindowHandles.Count)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "标签页顺序值不可超过最大值！");
                    return;
                }
                ChromeDriver chromeDriver = driver as ChromeDriver;
                FirefoxDriver fxDriver = driver as FirefoxDriver;
                ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
                if (chromeDriver != null)
                {
                    //标签页面顺序不同 CHROME浏览器特殊处理
                    string handle;
                    if (tabSeq == 0)
                        handle = windowHandles[tabSeq];
                    else
                        handle = windowHandles[windowHandles.Count - tabSeq];
                    driver.SwitchTo().Window(handle);
                }
                else
                {
                    //IE处理
                    string handle = windowHandles[tabSeq];
                    driver.SwitchTo().Window(handle);
                }
            }
            else if (TabKey.Expression != null)
            {
                bool flag = false;
                string currHandle = driver.CurrentWindowHandle;
                string tabKey = TabKey.Get(context);
                foreach (String handle in driver.WindowHandles)
                {
                    driver.SwitchTo().Window(handle);
                    if (driver.Title.Contains(tabKey) || driver.Url.Contains(tabKey))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag == false)
                {
                    SharedObject.Instance.Output(SharedObject.enOutputType.Error, "通过关键字切换标签页失败，请检查关键字！");
                    driver.SwitchTo().Window(currHandle);
                }
            }
        }
    }
}
