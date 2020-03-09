using Fluent;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugins.Shared.Library.Nuget;
using RPAStudio.Helpers;
using RPAStudio.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RPAStudio.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            _themes = new ObservableCollection<Theme>(ThemeManager.Themes);
            _currentTheme = ThemeManager.DetectTheme().Name;

            _languages = new ObservableCollection<CultureInfo>(App.SupportedLanguages);
            _currentLanguage = SettingsManager.GetUserLanguageOrDefault();

            (_autoSaveIsEnabled, _autoSaveSeconds) = SettingsManager.GetAutoSaveSettings();
        }


        private ObservableCollection<Theme> _themes;
        public ObservableCollection<Theme> Themes
        {
            get => _themes;
            set
            {
                if (value != _themes)
                {
                    _themes = value;
                    RaisePropertyChanged(nameof(Themes));
                }
            }
        }

        private string _currentTheme;
        public string CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (value != _currentTheme)
                {
                    _currentTheme = value;
                    RaisePropertyChanged(nameof(CurrentTheme));
                }
            }
        }

        private ObservableCollection<CultureInfo> _languages;
        public ObservableCollection<CultureInfo> Languages
        {
            get => _languages;
            set
            {
                if (_languages != value)
                {
                    _languages = value;
                    RaisePropertyChanged(nameof(Languages));
                }
            }
        }

        private string _currentLanguage;
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (value != _currentLanguage)
                {
                    _currentLanguage = value;
                    RaisePropertyChanged(nameof(CurrentLanguage));
                }
            }
        }

        private bool _autoSaveIsEnabled = true;
        /// <summary>
        /// 自动保存是否启用
        /// </summary>
        public bool AutoSaveIsEnabled
        {
            get => _autoSaveIsEnabled;
            set
            {
                if (value != _autoSaveIsEnabled)
                {
                    _autoSaveIsEnabled = value;
                    SettingsManager.SetAutoSaveEnabledValue(value);
                    RaisePropertyChanged(nameof(AutoSaveIsEnabled));
                }
            }
        }

        private int _autoSaveSeconds = 30;
        /// <summary>
        /// 自动保存时间
        /// </summary>
        public int AutoSaveSeconds
        {
            get => _autoSaveSeconds;
            set
            {
                if (value != _autoSaveSeconds)
                {
                    _autoSaveSeconds = value;
                    SettingsManager.SetAutoSaveSecondsValue(value);
                    RaisePropertyChanged(nameof(AutoSaveSeconds));
                }
            }
        }

        private RelayCommand _changeThemeCommand;
        public RelayCommand ChangeThemeCommand
        {
            get
            {
                if (_changeThemeCommand == null)
                {
                    _changeThemeCommand = new RelayCommand(() =>
                    {
                        var current = ThemeManager.DetectTheme().Name;
                        if (current != _currentTheme)
                        {
                            var theme = ThemeManager.GetTheme(_currentTheme);
                            if (theme != null)
                            {
                                SettingsManager.SetTheme(theme);
                                ThemeManager.ChangeTheme(Application.Current, theme);
                            }
                        }
                    });
                }
                return _changeThemeCommand;
            }
        }

        private RelayCommand _changeLanguageCommand;
        public RelayCommand ChangeLanguageCommand
        {
            get
            {
                if (_changeLanguageCommand == null)
                {
                    _changeLanguageCommand = new RelayCommand(() =>
                    {
                        var current = CultureInfo.GetCultureInfo(_currentLanguage);
                        if (current != null)
                        {
                            ViewModelLocator.Instance.Main.IsStartContentBusy = true;
                            var result = SettingsManager.SetLanguage(_currentLanguage);
                            ViewModelLocator.Instance.Main.IsStartContentBusy = false;
                            if (result)
                            {
                                var window = new RestartWindow();
                                window.Owner = Application.Current.MainWindow;
                                window.ShowDialog();
                            }
                        }
                    });
                }
                return _changeLanguageCommand;
            }
        }

        private RelayCommand _clearCacheCommand;
        public RelayCommand ClearCacheCommand => _clearCacheCommand ?? (_clearCacheCommand = new RelayCommand(() =>
        {
            NuGetPackageController.Instance.ClearInstalledPackages();
            System.Windows.Forms.AutoClosingMessageBox.Show("清除缓存成功");
        }));
    }
}
