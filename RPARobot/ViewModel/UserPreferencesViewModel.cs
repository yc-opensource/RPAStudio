using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RPARobot.Librarys;
using System.Windows;
using System.Xml;
using System;
using System.Windows.Controls;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UserPreferencesViewModel : ViewModelBase
    {
        public Page m_view { get; set; }

        /// <summary>
        /// Initializes a new instance of the UserPreferencesViewModel class.
        /// </summary>
        public UserPreferencesViewModel()
        {
        }


        private RelayCommand<RoutedEventArgs> _loadedCommand;

        /// <summary>
        /// Gets the LoadedCommand.
        /// </summary>
        public RelayCommand<RoutedEventArgs> LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand<RoutedEventArgs>(
                    p =>
                    {
                        m_view = (Page)p.Source;
                    }));
            }
        }

        public void LoadSettings()
        {
            //读取配置XML文件来初始化界面信息
            XmlDocument xmlDocument = new XmlDocument();
            var path = App.LocalRPAStudioDir + @"\Config\RPARobot.settings";
            xmlDocument.Load(path);
            var rootNode = xmlDocument.DocumentElement;
            var userSettingsElement = rootNode.SelectSingleNode("UserSettings") as XmlElement;

            //根据自启动项设置IsAutoRun值
            var isAutoRunElement = userSettingsElement.SelectSingleNode("IsAutoRun") as XmlElement;
            if (isAutoRunElement.InnerText.ToLower().Trim() == "true")
            {
                //注册表设置自启动
                Common.SetAutoRun(true);
                IsAutoRun = true;
            }
            else
            {
                ////注册表取消自启动
                Common.SetAutoRun(false);
                IsAutoRun = false;
            }

            //根据配置XML信息设置IsAutoOpenMainWindow
            var isAutoOpenMainWindowElement = userSettingsElement.SelectSingleNode("IsAutoOpenMainWindow") as XmlElement;
            if (isAutoOpenMainWindowElement.InnerText.ToLower().Trim() == "true")
            {
                IsAutoOpenMainWindow = true;
            }
            else
            {
                IsAutoOpenMainWindow = false;
            }

            if ((userSettingsElement.SelectSingleNode("IsEnableScreenRecorder") as XmlElement)?.InnerText.ToLower().Trim() == "true")
            {
                IsEnableScreenRecorder = true;
            }
            else
            {
                IsEnableScreenRecorder = false;
            }
            XmlElement fpsElement = userSettingsElement.SelectSingleNode("FPS") as XmlElement;
            XmlElement qualityElement = userSettingsElement.SelectSingleNode("Quality") as XmlElement;
            if (int.TryParse(fpsElement?.InnerText.Trim(), out var fps))
            {
                FPS = fps;
            }
            if (int.TryParse(qualityElement?.InnerText.Trim(), out var quality))
            {
                Quality = quality;
            }
            //if ((userSettingsElement.SelectSingleNode("IsEnableControlServer") as XmlElement).InnerText.ToLower().Trim() == "true")
            //{
            //    this.IsEnableControlServer = true;
            //}
            //else
            //{
            //    this.IsEnableControlServer = false;
            //}
            //XmlElement xmlElement4 = userSettingsElement.SelectSingleNode("ControlServerUri") as XmlElement;
            //this.ControlServerUri = xmlElement4.InnerText.Trim();
        }

        /// <summary>
        /// The <see cref="IsAutoRun" /> property's name.
        /// </summary>
        public const string IsAutoRunPropertyName = "IsAutoRun";

        private bool _isAutoRunProperty = false;

        /// <summary>
        /// Sets and gets the IsAutoRun property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAutoRun
        {
            get
            {
                return _isAutoRunProperty;
            }

            set
            {
                if (_isAutoRunProperty == value)
                {
                    return;
                }

                _isAutoRunProperty = value;
                RaisePropertyChanged(IsAutoRunPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsAutoOpenMainWindow" /> property's name.
        /// </summary>
        public const string IsAutoOpenMainWindowPropertyName = "IsAutoOpenMainWindow";

        private bool _isAutoOpenMainWindowProperty = false;

        /// <summary>
        /// Sets and gets the IsAutoOpenMainWindow property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAutoOpenMainWindow
        {
            get
            {
                return _isAutoOpenMainWindowProperty;
            }

            set
            {
                if (_isAutoOpenMainWindowProperty == value)
                {
                    return;
                }

                _isAutoOpenMainWindowProperty = value;
                RaisePropertyChanged(IsAutoOpenMainWindowPropertyName);
            }
        }



        private RelayCommand _resetSettingsCommand;

        /// <summary>
        /// Gets the ResetSettingsCommand.
        /// </summary>
        public RelayCommand ResetSettingsCommand
        {
            get
            {
                return _resetSettingsCommand
                    ?? (_resetSettingsCommand = new RelayCommand(
                    () =>
                    {
                        //重置默认设置
                        var ret = MessageBox.Show("确认重置为默认设置吗？", "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                        if (ret == MessageBoxResult.OK)
                        {
                            IsAutoRun = false;
                            IsAutoOpenMainWindow = true;

                            //立即生效
                            OkCommand.Execute(null);
                        }

                    }));
            }
        }



        private RelayCommand _okCommand;

        /// <summary>
        /// Gets the OkCommand.
        /// </summary>
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new RelayCommand(
                    () =>
                    {
                        //写入到配置XML中，执行可能的相应的操作
                        XmlDocument doc = new XmlDocument();
                        var path = App.LocalRPAStudioDir + @"\Config\RPARobot.settings";
                        doc.Load(path);
                        var rootNode = doc.DocumentElement;
                        var userSettingsElement = rootNode.SelectSingleNode("UserSettings") as XmlElement;

                        //根据自启动项设置IsAutoRun值
                        var isAutoRunElement = userSettingsElement.SelectSingleNode("IsAutoRun") as XmlElement;
                        isAutoRunElement.InnerText = IsAutoRun.ToString();

                        var isAutoOpenMainWindowElement = userSettingsElement.SelectSingleNode("IsAutoOpenMainWindow") as XmlElement;
                        isAutoOpenMainWindowElement.InnerText = IsAutoOpenMainWindow.ToString();

                        (userSettingsElement.SelectSingleNode("IsEnableScreenRecorder") as XmlElement).InnerText = IsEnableScreenRecorder.ToString();
                        (userSettingsElement.SelectSingleNode("FPS") as XmlElement).InnerText = FPS.ToString();
                        (userSettingsElement.SelectSingleNode("Quality") as XmlElement).InnerText = Quality.ToString();

                        doc.Save(path);

                        Common.SetAutoRun(IsAutoRun);
                    }));
            }
        }

        /// <summary>
        /// 是否启用屏幕录制 TODO 设置项
        /// </summary>
        private bool _isEnableScreenRecorder = true;
        public bool IsEnableScreenRecorder
        {
            get => _isEnableScreenRecorder;
            set
            {
                if (value != _isEnableScreenRecorder)
                {
                    _isEnableScreenRecorder = value;
                    RaisePropertyChanged(nameof(IsEnableScreenRecorder));
                }
            }
        }

        /// <summary>
        /// 视频质量
        /// </summary>
        private int _quality = 50;
        public int Quality
        {
            get => _quality;
            set
            {
                if (value != _quality)
                {
                    _quality = value;
                    RaisePropertyChanged(nameof(Quality));
                }
            }
        }

        /// <summary>
        /// 视频帧数
        /// </summary>
        private int _fps = 30;
        public int FPS
        {
            get => _fps;
            set
            {
                if (value != _fps)
                {
                    _fps = value;
                    RaisePropertyChanged(nameof(FPS));
                }
            }
        }
    }
}