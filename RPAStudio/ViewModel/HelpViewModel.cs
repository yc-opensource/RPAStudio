using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using RPAStudio.Librarys;
using RPAUpdate.Librarys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace RPAStudio.ViewModel
{
    public class HelpViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HttpDownloadFile m_downloader { get; set; }

        //RPAUpgradeClientConfig.xml相关配置
        private string m_rpaUpgradeServerConfigUrl { get; set; }
        private List<string> m_currentVersionUpdateLogList { get; set; } = new List<string>();

        //RPAUpgradeServerConfig.xml相关配置
        private string m_autoUpgradePackpageVersion { get; set; }
        private string m_autoUpgradePackpageMd5 { get; set; }
        private string m_autoUpgradePackpageUrl { get; set; }
        private List<string> m_latestVersionUpdateLogList { get; set; } = new List<string>();

        public HelpViewModel()
        {
            _helps = new ObservableCollection<HelpItem>(new List<HelpItem> {
                new HelpItem("","使用文档","查看产品使用说明书","http://baidu.com"),
                new HelpItem("","发行说明","查看发行版的完整历史记录和发行说明","http://baidu.com"),
            });

            InitRPAUpgradeClientConfig();

            InitCurrentVersionInfo();
        }

        private string _currentVersionName;
        public string CurrentVersionName
        {
            get => _currentVersionName;
            set
            {
                if (value != _currentVersionName)
                {
                    _currentVersionName = value;
                    RaisePropertyChanged(nameof(CurrentVersionName));
                }
            }
        }

        private string _latestVersionName;
        public string LatestVersionName
        {
            get => _latestVersionName;
            set
            {
                if (value != _latestVersionName)
                {
                    _latestVersionName = value;
                    RaisePropertyChanged(nameof(LatestVersionName));
                }
            }
        }

        private string _upgradeButtonText = Localization.strings.help_check_for_update_btn_text;
        public string UpgradeButtonText
        {
            get => _upgradeButtonText;
            set
            {
                if (value != _upgradeButtonText)
                {
                    _upgradeButtonText = value;
                    RaisePropertyChanged(nameof(UpgradeButtonText));
                }
            }
        }

        private bool _isNeedUpgrade = false;
        public bool IsNeedUpgrade
        {
            get
            {
                return _isNeedUpgrade;
            }

            set
            {
                if (_isNeedUpgrade != value)
                {
                    _isNeedUpgrade = value;
                    RaisePropertyChanged(nameof(IsNeedUpgrade));
                }
                ViewModelLocator.Instance.Main.IsNeedUpgrade = value;
            }
        }

        private bool _isChecking = false;
        public bool IsChecking
        {
            get => _isChecking;
            set
            {
                if (value != _isChecking)
                {
                    _isChecking = value;
                    RaisePropertyChanged(nameof(IsChecking));
                    Common.RunInUI(CheckUpgradeCommand.RaiseCanExecuteChanged);
                }
            }
        }

        private bool _isShowLatestVersionUpdateLog;
        public bool IsShowLatestVersionUpdateLog
        {
            get => _isShowLatestVersionUpdateLog;
            set
            {
                if (value != _isShowLatestVersionUpdateLog)
                {
                    _isShowLatestVersionUpdateLog = value;
                    RaisePropertyChanged(nameof(IsShowLatestVersionUpdateLog));
                }
            }
        }

        private string _latestVersionUpdateLog;
        public string LatestVersionUpdateLog
        {
            get => _latestVersionUpdateLog;
            set
            {
                if (value != _latestVersionUpdateLog)
                {
                    _latestVersionUpdateLog = value;
                    RaisePropertyChanged(nameof(LatestVersionUpdateLog));
                }
            }
        }

        private bool _isDownloadButtonVisible = false;
        public bool IsDownloadButtonVisible
        {
            get => _isDownloadButtonVisible;
            set
            {
                if (value != _isDownloadButtonVisible)
                {
                    _isDownloadButtonVisible = value;
                    RaisePropertyChanged(nameof(IsDownloadButtonVisible));
                }
            }
        }

        private string _downloadButtonText = Localization.strings.help_download_btn_text;
        public string DownloadButtonText
        {
            get => _downloadButtonText;
            set
            {
                if (value != _downloadButtonText)
                {
                    _downloadButtonText = value;
                    RaisePropertyChanged(nameof(DownloadButtonText));
                }
            }
        }

        private ObservableCollection<HelpItem> _helps;
        public ObservableCollection<HelpItem> Helps
        {
            get => _helps;
            set
            {
                if (value != _helps)
                {
                    _helps = value;
                    RaisePropertyChanged(nameof(Helps));
                }
            }
        }

        private bool _isDoUpgradeEnable = true;
        public bool IsDoUpgradeEnable
        {
            get => _isDoUpgradeEnable;
            set
            {
                if (value != _isDoUpgradeEnable)
                {
                    _isDoUpgradeEnable = value;
                    RaisePropertyChanged(nameof(IsDoUpgradeEnable));
                }
            }
        }


        private RelayCommand _checkUpgradeCommand;
        public RelayCommand CheckUpgradeCommand => _checkUpgradeCommand ?? (_checkUpgradeCommand = new RelayCommand(() =>
        {
            //检查是否要更新
            Task.Run(() =>
            {
                UpgradeButtonText = Localization.strings.help_checking_for_update_btn_text;
                IsChecking = true;

                if (InitRPAUpgradeServerConfig())
                {
                    InitLatestVersionInfo();

                    IsChecking = false;

                    if (IsNeedUpgrade)
                    {
                        // 隐藏检查更新按钮，显示下载按钮
                        IsDownloadButtonVisible = true;
                    }
                    else
                    {
                        IsDownloadButtonVisible = false;
                        LatestVersionUpdateLog = Localization.strings.help_no_need_to_update;
                    }

                    IsShowLatestVersionUpdateLog = true;
                }

                IsChecking = false;
                UpgradeButtonText = Localization.strings.help_check_for_update_btn_text;
            });
        }, () => !IsChecking));

        private RelayCommand _doUpgradeCommand;
        public RelayCommand DoUpgradeCommand => _doUpgradeCommand ?? (_doUpgradeCommand = new RelayCommand(() =>
        {
            //判断升级包是否已经下载好了，如果已经下载，则直接安装
            var hasDownload = false;
            var originFileName = Common.GetFileNameFromUrl(m_autoUpgradePackpageUrl);
            var path = App.LocalRPAStudioDir + string.Format(@"\Update\{0}", originFileName);
            if (File.Exists(path) && Common.GetMD5HashFromFile(path).ToLower() == m_autoUpgradePackpageMd5.ToLower())
            {
                hasDownload = true;
            }

            //if (hasDownload)
            //{
            //    IsShowProgressBar = true;
            //    DownloadingProgressValue = 100;

            //    DoInstallUpgradePackage(path);
            //}
            //else
            //{
            //    IsShowProgressBar = false;
            //    var thread = new Thread(DownloadThread);//创建一个线程
            //    thread.Start();//开始一个线程
            //}
        }, () => IsDoUpgradeEnable));


        private void InitCurrentVersionInfo()
        {
            CurrentVersionName = $"MoRPA Studio {Common.GetProgramVersion()}";
        }

        private void InitLatestVersionInfo()
        {
            Version currentVersion = new Version(Common.GetProgramVersion());
            Version latestVersion = new Version(m_autoUpgradePackpageVersion);

            if (latestVersion > currentVersion)
            {
                IsNeedUpgrade = true;
                LatestVersionName = "v" + m_autoUpgradePackpageVersion;

                LatestVersionUpdateLog = "";
                foreach (var item in m_latestVersionUpdateLogList)
                {
                    LatestVersionUpdateLog += " ● " + item + Environment.NewLine;
                }
            }
            else
            {
                IsNeedUpgrade = false;
            }

        }

        private bool InitRPAUpgradeServerConfig()
        {
            bool ret = true;
            var rpaUpgradeServerConfig = HttpRequest.Get(m_rpaUpgradeServerConfigUrl);

            if (!string.IsNullOrEmpty(rpaUpgradeServerConfig))
            {
                m_latestVersionUpdateLogList.Clear();

                XmlDocument doc = new XmlDocument();

                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rpaUpgradeServerConfig)))
                {
                    ms.Flush();
                    ms.Position = 0;
                    doc.Load(ms);
                    ms.Close();
                }

                var rootNode = doc.DocumentElement;
                var autoUpgradePackpageElement = rootNode.SelectSingleNode("AutoUpgradePackpage") as XmlElement;

                m_autoUpgradePackpageVersion = autoUpgradePackpageElement.GetAttribute("Version");
                m_autoUpgradePackpageMd5 = autoUpgradePackpageElement.GetAttribute("Md5");
                m_autoUpgradePackpageUrl = autoUpgradePackpageElement.GetAttribute("Url");

                var updateLogElement = rootNode.SelectSingleNode("UpdateLog");
                var items = updateLogElement.SelectNodes("Item");
                foreach (var item in items)
                {
                    var text = (item as XmlElement).InnerText;
                    m_latestVersionUpdateLogList.Add(text);
                }
            }
            else
            {
                ret = false;
                Logger.Error("获取升级配置文件失败，请检查!url=" + m_rpaUpgradeServerConfigUrl, logger);
            }


            return ret;
        }

        private void InitRPAUpgradeClientConfig()
        {
            m_currentVersionUpdateLogList.Clear();

            XmlDocument doc = new XmlDocument();

            using (var ms = new MemoryStream(Properties.Resources.RPAUpgradeClientConfig))
            {
                ms.Flush();
                ms.Position = 0;
                doc.Load(ms);
                ms.Close();
            }

            var rootNode = doc.DocumentElement;
            var rpaUpgradeServerConfigElement = rootNode.SelectSingleNode("RPAUpgradeServerConfig") as XmlElement;
            m_rpaUpgradeServerConfigUrl = rpaUpgradeServerConfigElement.GetAttribute("Url");

            var updateLogElement = rootNode.SelectSingleNode("UpdateLog");
            var items = updateLogElement.SelectNodes("Item");
            foreach (var item in items)
            {
                var text = (item as XmlElement).InnerText;
                m_currentVersionUpdateLogList.Add(text);
            }
        }
    }

    public class HelpItem : ViewModelBase
    {
        private string _image;
        public string Image
        {
            get => _image;
            set
            {
                if (value != _image)
                {
                    _image = value;
                    RaisePropertyChanged(nameof(Image));
                }
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged(nameof(Title));
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    RaisePropertyChanged(nameof(Description));
                }
            }
        }

        private string _link;
        public string Link
        {
            get => _link;
            set
            {
                if (value != _link)
                {
                    _link = value;
                    RaisePropertyChanged(nameof(Link));
                }
            }
        }


        private RelayCommand _clickCommand;
        public RelayCommand ClickCommand => _clickCommand ?? (_clickCommand = new RelayCommand(() =>
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = Link;
            proc.Start();
        }));

        public HelpItem(string image, string title, string description, string link)
        {
            _image = image;
            _title = title;
            _description = description;
            _link = link;
        }
    }
}
