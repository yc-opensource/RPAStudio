using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.CSharp.RuntimeBinder;
using Plugins.Shared.Library.UiAutomation;
using RPAStudio.Librarys;
using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RPAStudio.ViewModel
{
    public class RecordingViewModel : ViewModelBase
    {
        public Window m_view;


        private bool _isRecorded;
        public bool IsRecorded
        {
            get => _isRecorded;
            set
            {
                if (value != _isRecorded)
                {
                    _isRecorded = value;
                    RaisePropertyChanged(nameof(IsRecorded));
                }
            }
        }


        private RelayCommand<RoutedEventArgs> _loadedCommand;
        public RelayCommand<RoutedEventArgs> LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand<RoutedEventArgs>(p =>
        {
            m_view = p.Source as Window;
            UiElement.IsRecordingWindowOpened = true;
        }));

        private RelayCommand _unloadedCommand;
        public RelayCommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(() =>
        {
            UiElement.IsRecordingWindowOpened = false;
        }));

        private RelayCommand _saveAndExitCommand;
        public RelayCommand SaveAndExitCommand => _saveAndExitCommand ?? (_saveAndExitCommand = new RelayCommand(() =>
        {
            if (ViewModelLocator.Instance.Dock.ActiveDocument != null &&
                !ViewModelLocator.Instance.Dock.ActiveDocument.IsReadOnly)
            {
                var modelService = ViewModelLocator.Instance.Dock.ActiveDocument.WorkflowDesignerInstance.Context.Services.GetService<ModelService>();
                var implementation = modelService.Root.Properties["Implementation"]?.Value;
                if (implementation == null)
                {
                    modelService.Root.Content.SetValue(new Sequence());
                    implementation = modelService.Root.Properties["Implementation"].Value;
                }

                foreach (var stuActivityInfo in m_activityRecordingList)
                {
                    stuActivityInfo.preAction?.Invoke();
                    var modelItem = implementation.AddActivity(stuActivityInfo.activity, -1);
                    if (modelItem != null)
                    {
                        if (modelItem.IsFlowStep())
                        {
                            modelItem = modelItem.Properties["Action"].Value;
                        }
                        stuActivityInfo.postAction?.Invoke(modelItem);
                    }
                }
            }
        }));

        private RelayCommand<CancelEventArgs> _closingCommand;
        public RelayCommand<CancelEventArgs> ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand<CancelEventArgs>(p =>
        {
            var flag = true;
            if (IsRecorded)
            {
                m_view.Topmost = false;
                if (MessageBox.Show(Application.Current.MainWindow, "录制结果未保存，确定退出吗？", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.No)
                {
                    flag = false;
                    this.m_view.Topmost = true;
                }
            }
            if (!flag)
            {
                p.Cancel = true;
            }
        }));

        private RelayCommand _mouseLeftClickCommand;
        public RelayCommand MouseLeftClickCommand => _mouseLeftClickCommand ?? (_mouseLeftClickCommand = new RelayCommand(() =>
        {
            m_view.WindowState = WindowState.Minimized;
            UiElement.OnSelected = new UiElement.UiElementSelectedEventHandler(UiElement_OnMouseLeftClickSelected);
            UiElement.StartElementHighlight();
        }));

        private void UiElement_OnMouseLeftClickSelected(UiElement uiElement)
        {
            uiElement.MouseClick(null);
            this.DoMouseSelect(uiElement, "ClickActivity", delegate (object activity)
            {
                if (m_callSiteClick == null)
                {
                    m_callSiteClick = CallSite<Action<CallSite, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetMouseLeftClick", null, typeof(RecordingViewModel), new CSharpArgumentInfo[]
                    {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                    }));
                }
                m_callSiteClick.Target(m_callSiteClick, activity);
            });
        }

        private List<stuActivityInfo> m_activityRecordingList = new List<stuActivityInfo>();

        private CallSite<Action<CallSite, object>> m_callSiteClick = null;
        private CallSite<Action<CallSite, Action<object>, object>> m_callSite0 = null;
        private CallSite<Func<CallSite, object, string, object>> m_callSite1 = null;
        private CallSite<Func<CallSite, object, string, object>> m_callSite2 = null;
        private CallSite<Func<CallSite, object, Visibility, object>> m_callSite3 = null;
        private CallSite<Func<CallSite, object, int, object>> m_callSite4 = null;
        private CallSite<Func<CallSite, object, int, object>> m_callSite5 = null;
        private CallSite<Func<CallSite, object, int, object>> m_callSite6 = null;
        private CallSite<Func<CallSite, object, int, object>> m_callSite7 = null;
        private CallSite<Func<CallSite, object, int, object>> m_callSite8 = null;
        private CallSite<Func<CallSite, object, int, object>> m_callSite9 = null;
        private CallSite<Func<CallSite, object, Activity>> m_callSite10 = null;
        private void DoMouseSelect(UiElement uiElement, string type, Action<object> action = null)
        {
            IsRecorded = true;
            m_view.WindowState = WindowState.Normal;
            m_view.Topmost = true;

            var activity = Activator.CreateInstance(Type.GetType($"RPA.UIAutomation.Activities.Mouse.{type},RPA.UIAutomation.Activities"));
            if (activity != null)
            {
                if (m_callSite0 == null)
                {
                    m_callSite0 = CallSite<Action<CallSite, Action<object>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Invoke", null, typeof(RecordingViewModel), new CSharpArgumentInfo[]
                    {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                    }));
                }
                m_callSite0.Target(m_callSite0, action, activity);
            }
            if (m_callSite1 == null)
            {
                m_callSite1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SourceImgPath", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite1.Target(m_callSite0, activity, uiElement.CaptureInformativeScreenshotToFile(null));
            if (m_callSite2 == null)
            {
                m_callSite2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Selector", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite2.Target(m_callSite2, activity, uiElement.Selector);
            if (m_callSite3 == null)
            {
                m_callSite3 = CallSite<Func<CallSite, object, Visibility, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "visibility", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
                }));
            }
            m_callSite3.Target(m_callSite3, activity, Visibility.Visible);
            if (m_callSite4 == null)
            {
                m_callSite4 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "offsetX", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite4.Target(m_callSite4, activity, uiElement.GetClickablePoint().X);
            if (m_callSite5 == null)
            {
                m_callSite5 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "offsetY", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite5.Target(m_callSite5, activity, uiElement.GetClickablePoint().Y);
            if (m_callSite6 == null)
            {
                m_callSite6 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Left", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite6.Target(m_callSite6, activity, uiElement.BoundingRectangle.Left);
            if (m_callSite7 == null)
            {
                m_callSite7 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Right", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite7.Target(m_callSite7, activity, uiElement.BoundingRectangle.Right);
            if (m_callSite8 == null)
            {
                m_callSite8 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Top", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite8.Target(m_callSite8, activity, uiElement.BoundingRectangle.Top);
            if (m_callSite9 == null)
            {
                m_callSite9 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Bottom", typeof(RecordingViewModel), new CSharpArgumentInfo[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
                }));
            }
            m_callSite9.Target(m_callSite9, activity, uiElement.BoundingRectangle.Bottom);
            string append_displayName = string.Concat(new string[]
            {
                " \"",
                uiElement.ProcessName,
                " ",
                uiElement.Name,
                "\""
            });
            stuActivityInfo item = default;
            if (m_callSite10 == null)
            {
                m_callSite10 = CallSite<Func<CallSite, object, Activity>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(Activity), typeof(RecordingViewModel)));
            }
            item.activity = m_callSite10.Target(m_callSite10, activity);
            item.postAction = delegate (ModelItem modelItem)
            {
                modelItem.Properties["DisplayName"].SetValue(modelItem.Properties["DisplayName"].Value + append_displayName);
            };
            m_activityRecordingList.Add(item);
        }

        private struct stuActivityInfo
        {
            public Activity activity;
            public Action preAction;
            public Action<ModelItem> postAction;
        }
    }
}
