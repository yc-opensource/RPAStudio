﻿using System;
using System.Activities;
using System.ComponentModel;
using System.Reflection;
using Plugins.Shared.Library;
using Microsoft.Office.Interop.Word;

namespace RPA.Integration.Activities.WordPlugins
{
    [Designer(typeof(MarkLinkDesigner))]
    public sealed class MarkLink : AsyncCodeActivity
    {
        public MarkLink()
        {
        }

        [RequiredArgument]
        [OverloadGroup("Picture")]
        [Category("图片")]
        [DisplayName("图片链接")]
        [Browsable(true)]
        public InArgument<string> Pic
        {
            get;set;
        }

        [RequiredArgument]
        [OverloadGroup("BookMark")]
        [Category("书签")]
        [DisplayName("书签名称")]
        [Browsable(true)]
        public InArgument<string> BookMark
        {
            get;set;
        }

        [RequiredArgument]
        [OverloadGroup("Link")]
        [Category("超链接")]
        [DisplayName("超链接名称")]
        [Browsable(true)]
        public InArgument<string> LinkName
        {
            get;set;
        }

        [RequiredArgument]
        [OverloadGroup("Link")]
        [Category("超链接")]   
        [DisplayName("超链接标签")]
        [Browsable(true)]
        public InArgument<string> LinkMark
        {
            get;set;
        }

        [RequiredArgument]
        [OverloadGroup("Link")]
        [Category("超链接")]
        [DisplayName("超链接地址")]
        [Browsable(true)]
        public InArgument<string> LinkAddr
        {
            get;set;
        }


        //设置属性可见/不可见
        private void SetPropertyVisibility(object obj, string propertyName, bool visible)
        {
            try
            {
                Type type = typeof(BrowsableAttribute);
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
                AttributeCollection attrs = props[propertyName].Attributes;
                FieldInfo fld = type.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic);
                fld.SetValue(attrs[type], visible);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex);
            }
        }

        //设置属性只读
        private void SetPropertyReadOnly(object obj, string propertyName, bool readOnly)
        {
            try
            {
                Type type = typeof(ReadOnlyAttribute);
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
                AttributeCollection attrs = props[propertyName].Attributes;
                FieldInfo fld = type.GetField("ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                fld.SetValue(attrs[type], readOnly);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex);
            }
        }

        [Browsable(false)]
        public string icoPath { get { return "pack://application:,,,/RPA.Integration.Activities;Component/Resources/Word/mark.png"; } }

        [Browsable(false)]
        public string ClassName { get { return "MarkLink"; } }
        private delegate string runDelegate();
        private runDelegate m_Delegate;
        public string Run()
        {
            return ClassName;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            PropertyDescriptor property = context.DataContext.GetProperties()[WordCreate.GetWordAppTag];
            Application wordApp = property.GetValue(context.DataContext) as Application;
            try
            {
                string linkName = LinkName.Get(context);
                string linkMark = LinkMark.Get(context);
                string linkAddr = LinkAddr.Get(context);
                string bookMark = BookMark.Get(context);
                string pic = Pic.Get(context);

                if (linkName != null)
                {
                    Hyperlinks links = wordApp.Selection.Hyperlinks;
                    links.Add(wordApp.Selection.Range, linkAddr, linkMark, "", linkName, linkMark);
                }
                if (bookMark != null)
                {
                    Bookmarks bookmarks = wordApp.Selection.Bookmarks;
                    bookmarks.Add(bookMark);
                }
                if (pic != null)
                {
                    InlineShapes lineshapes = wordApp.Selection.InlineShapes;
                    InlineShape lineshape = lineshapes.AddPicture(pic);
                }
            }
            catch (Exception e)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Error, "Word执行过程出错", e.Message);
                CommonVariable.realaseProcessExit(wordApp);
            }

            m_Delegate = new runDelegate(Run);
            return m_Delegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }
    }
}
