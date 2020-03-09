using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using Plugins.Shared.Library;
using System;
using System.Activities;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace RPA.UIAutomation.Activities.Browser
{
    [Designer(typeof(NewTabDesigner))]
    public sealed class NewTab : AsyncCodeActivity
    {
        static NewTab()
        {
        }

        [Browsable(false)]
        public string icoPath
        {
            get
            {
                return @"pack://application:,,,/RPA.UIAutomation.Activities;Component/Resources/Browser/NewTab.png";
            }
        }


        [Category("输入")]
        [Browsable(true)]
        [DisplayName("浏览器Browser")]
        [Description("该字段仅支持Browser变量")]
        public InArgument<Browser> currBrowser { get; set; }

        [Category("输入")]
        [Browsable(true)]
        [DisplayName("新标签地址")]
        [RequiredArgument]
        public InArgument<string> UrlString { get; set; }



        public string ClassName { get { return "NewTab"; } }
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
                        SharedObject.Instance.Output(SharedObject.enOutputType.Error, "活动流程传递的浏览器变量为空，请检查!");
                        m_Delegate = new runDelegate(Run);
                        return m_Delegate.BeginInvoke(callback, state);
                    }
                    Browser getBrowser = property.GetValue(context.DataContext) as Browser;
                    IWebDriver driver = getBrowser.getICFBrowser();
                    NewTabFunc(context, driver);
                }
                else
                {
                    if (thisBrowser.getICFBrowser() != null)
                    {
                        IWebDriver driver = thisBrowser.getICFBrowser() as ChromeDriver;
                        NewTabFunc(context, driver);
                    }
                }
            }
            catch(Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "新建标签页失败!", e);
                throw e;
            }


            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
        private void NewTabFunc(AsyncCodeActivityContext context, IWebDriver driver)
        {
            ChromeDriver chromeDriver = driver as ChromeDriver;
            InternetExplorerDriver ieDriver = driver as InternetExplorerDriver;
            FirefoxDriver fxDriver = driver as FirefoxDriver;
            string jsCode = "window.open('" + UrlString.Get(context) + "');";
            string buff = null;
            if (chromeDriver != null)
                chromeDriver.ExecuteScript(jsCode, buff);
            else if (ieDriver != null)
                ieDriver.ExecuteScript(jsCode, buff);
            else if (fxDriver != null)
                fxDriver.ExecuteScript(jsCode, buff);

            bool flag = false;
            string currHandle = driver.CurrentWindowHandle;
            foreach (String handle in driver.WindowHandles)
            {
                driver.SwitchTo().Window(handle);
                string[] str1 = Regex.Split(driver.Url, "//", RegexOptions.IgnoreCase);
                string[] str2 = Regex.Split(UrlString.Get(context), "//", RegexOptions.IgnoreCase);
                try
                {
                    if (string.Equals(str1[1].Trim('/'), str2[1].Trim('/')))
                    {
                        flag = true;
                        break;
                    }
                }catch
                {
                    return;
                }
        
            }
            if(!flag)
                driver.SwitchTo().Window(currHandle);
        }
    }
}
