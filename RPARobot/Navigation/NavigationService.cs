using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using RPARobot.ViewModel;
using RPARobot.Windows;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPARobot.Navigation
{
    public class NavigationService : ViewModelBase, INavigationService
    {
        public const string BackAction = "Back";
        public const string NextAction = "Next";

        private readonly Stack<string> _histories;
        private readonly ConcurrentDictionary<string, Uri> _pages;

        private string _currentPageKey;
        public string CurrentPageKey
        {
            get => _currentPageKey;
            private set => Set(() => CurrentPageKey, ref _currentPageKey, value);
        }

        public NavigationService()
        {
            _pages = new ConcurrentDictionary<string, Uri>();
            _histories = new Stack<string>();
        }

        public object Parameter { get; private set; }

        public void GoBack()
        {
            if (_histories.Count > 1)
            {
                var last = _histories.Pop();
                NavigateTo(last, BackAction);
            }
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, NextAction);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            if (!_pages.ContainsKey((pageKey)))
            {
                throw new KeyNotFoundException($"未找到页面{pageKey}，请检查！");
            }
            var frame = GetDescendantFromName(Application.Current.MainWindow, "MainFrame") as Frame;
            if (frame != null)
            {
                frame.Source = _pages[pageKey];
            }

            if (parameter?.ToString() == NextAction)
            {
                _histories.Push(pageKey);
            }
            Parameter = parameter;
            CurrentPageKey = pageKey;
        }

        public void AddPage(string key, Uri pagePath)
        {
            _pages.AddOrUpdate(key, pagePath, (_, __) => pagePath);
        }

        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
            {
                return null;
            }

            for (var i = 0; i < count; i++)
            {
                var frameworkElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (frameworkElement != null)
                {
                    if (frameworkElement.Name == name)
                    {
                        return frameworkElement;
                    }

                    frameworkElement = GetDescendantFromName(frameworkElement, name);
                    if (frameworkElement != null)
                    {
                        return frameworkElement;
                    }
                }
            }
            return null;
        }
    }
}
