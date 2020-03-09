using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NuGet;
using Plugins.Shared.Library;
using RPARobot.Executor;
using RPARobot.Librarys;
using RPARobot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RPARobot.ViewModel
{
    public class FlowViewModel : ViewModelBase
    {
        public string PackagesDir { get; set; }

        public string InstalledPackagesDir { get; set; }

        public FFmpegService FFmpegService { get; set; }

        //public ControlServerService ControlServerService { get; set; }

        public FlowViewModel()
        {
            var commonApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            PackagesDir = commonApplicationData + @"\MoRPA\Packages";//机器人默认读取nupkg包的位置
            InstalledPackagesDir = commonApplicationData + @"\MoRPA\InstalledPackages";//机器人默认读取nupkg包的位置

            Messenger.Default.Register<RunManager>(this, "BeginRun", BeginRun);
            Messenger.Default.Register<RunManager>(this, "EndRun", EndRun);
        }

        private ObservableCollection<PackageItem> _packageItemsProperty = new ObservableCollection<PackageItem>();

        /// <summary>
        /// Sets and gets the PackageItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<PackageItem> PackageItems
        {
            get
            {
                return _packageItemsProperty;
            }

            set
            {
                if (_packageItemsProperty == value)
                {
                    return;
                }

                _packageItemsProperty = value;
                RaisePropertyChanged(nameof(PackageItems));
            }
        }

        private bool _isWorkflowRunningProperty = false;

        /// <summary>
        /// Sets and gets the IsWorkflowRunning property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsWorkflowRunning
        {
            get
            {
                return _isWorkflowRunningProperty;
            }

            set
            {
                if (_isWorkflowRunningProperty == value)
                {
                    return;
                }

                _isWorkflowRunningProperty = value;
                RaisePropertyChanged(nameof(IsWorkflowRunning));
            }
        }

        private string _workflowRunningNameProperty = "";

        /// <summary>
        /// Sets and gets the WorkflowRunningName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WorkflowRunningName
        {
            get
            {
                return _workflowRunningNameProperty;
            }

            set
            {
                if (_workflowRunningNameProperty == value)
                {
                    return;
                }

                _workflowRunningNameProperty = value;
                RaisePropertyChanged(nameof(WorkflowRunningName));
            }
        }

        private string _searchTextProperty = "";

        /// <summary>
        /// Sets and gets the SearchText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SearchText
        {
            get
            {
                return _searchTextProperty;
            }

            set
            {
                if (_searchTextProperty == value)
                {
                    return;
                }

                _searchTextProperty = value;
                RaisePropertyChanged(nameof(SearchText));

                DoSearch();
            }
        }

        private bool _isSearchResultEmptyProperty = false;

        /// <summary>
        /// Sets and gets the IsSearchResultEmpty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSearchResultEmpty
        {
            get
            {
                return _isSearchResultEmptyProperty;
            }

            set
            {
                if (_isSearchResultEmptyProperty == value)
                {
                    return;
                }

                _isSearchResultEmptyProperty = value;
                RaisePropertyChanged(nameof(IsSearchResultEmpty));
            }
        }

        private string _workflowRunningStatusProperty = "";

        /// <summary>
        /// Sets and gets the WorkflowRunningStatus property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WorkflowRunningStatus
        {
            get
            {
                return _workflowRunningStatusProperty;
            }

            set
            {
                if (_workflowRunningStatusProperty == value)
                {
                    return;
                }

                _workflowRunningStatusProperty = value;
                RaisePropertyChanged(nameof(WorkflowRunningStatus));
            }
        }

        private string _workflowRunningToolTip = "";
        public string WorkflowRunningToolTip
        {
            get
            {
                return _workflowRunningToolTip;
            }
            set
            {
                if (_workflowRunningToolTip != value)
                {
                    _workflowRunningToolTip = value;
                    RaisePropertyChanged(nameof(WorkflowRunningToolTip));
                }
            }
        }

        private RelayCommand _onLoaded;
        public RelayCommand OnLoaded => _onLoaded ?? (_onLoaded = new RelayCommand(() =>
        {
            RefreshAllPackages();
        }));

        private RelayCommand _refreshCommand;

        /// <summary>
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(
                    () =>
                    {
                        RefreshAllPackages();
                    }));
            }
        }

        private RelayCommand _stopCommand;

        /// <summary>
        /// Gets the StopCommand.
        /// </summary>
        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand
                    ?? (_stopCommand = new RelayCommand(
                    () =>
                    {
                        ViewModelLocator.Instance.Main.m_runManager.Stop();
                    },
                    () => true));
            }
        }

        public void RefreshAllPackages()
        {
            PackageItems.Clear();

            var repo = PackageRepositoryFactory.Default.CreateRepository(PackagesDir);
            var pkgList = repo.GetPackages();

            var pkgSet = new SortedSet<string>();
            foreach (var pkg in pkgList)
            {
                //通过set去重
                pkgSet.Add(pkg.Id);
            }

            Dictionary<string, IPackage> installedPkgDict = new Dictionary<string, IPackage>();

            var packageManager = new PackageManager(repo, InstalledPackagesDir);
            foreach (IPackage pkg in packageManager.LocalRepository.GetPackages())
            {
                installedPkgDict[pkg.Id] = pkg;
            }

            foreach (var name in pkgSet)
            {
                var item = new PackageItem();
                item.Name = name;

                var version = repo.FindPackagesById(name).Max(p => p.Version);
                item.Version = version.ToString();

                var pkgNameList = repo.FindPackagesById(name);
                foreach (var i in pkgNameList)
                {
                    item.VersionList.Add(i.Version.ToString());
                }

                bool isNeedUpdate = false;
                if (installedPkgDict.ContainsKey(item.Name))
                {
                    var installedVer = installedPkgDict[item.Name].Version;
                    if (version > installedVer)
                    {
                        isNeedUpdate = true;
                    }
                }
                else
                {
                    isNeedUpdate = true;
                }
                item.IsNeedUpdate = isNeedUpdate;

                var pkg = repo.FindPackage(name, version);
                item.Package = pkg;
                var publishedTime = pkg.Published.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                item.ToolTip = string.Format("名称：{0}\r\n版本：{1}\r\n发布说明：{2}\r\n项目描述：{3}\r\n发布时间：{4}", item.Name, item.Version, pkg.ReleaseNotes, pkg.Description, (publishedTime == null ? "未知" : publishedTime));

                if (IsWorkflowRunning && item.Name == WorkflowRunningName)
                {
                    item.IsRunning = true;//如果当前该包工程已经在运行，则要设置IsRunning
                }

                PackageItems.Add(item);
            }


            DoSearch();
        }

        private void DoSearch()
        {
            var searchContent = SearchText.Trim();
            if (string.IsNullOrEmpty(searchContent))
            {
                //还原起始显示
                foreach (var item in PackageItems)
                {
                    item.IsSearching = false;
                }

                foreach (var item in PackageItems)
                {
                    item.SearchText = searchContent;
                }

                IsSearchResultEmpty = false;
            }
            else
            {
                //根据搜索内容显示

                foreach (var item in PackageItems)
                {
                    item.IsSearching = true;
                }

                //预先全部置为不匹配
                foreach (var item in PackageItems)
                {
                    item.IsMatch = false;
                }


                foreach (var item in PackageItems)
                {
                    item.ApplyCriteria(searchContent);
                }

                IsSearchResultEmpty = true;
                foreach (var item in PackageItems)
                {
                    if (item.IsMatch)
                    {
                        IsSearchResultEmpty = false;
                        break;
                    }
                }

            }
        }

        private void BeginRun(RunManager obj)
        {
            SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "流程运行开始……", null);
            if (this.FFmpegService != null)
            {
                this.FFmpegService.StopCaptureScreen();
                this.FFmpegService = null;
            }
            if (ViewModelLocator.Instance.UserPreferences.IsEnableScreenRecorder)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "屏幕录像开始……", null);
                var screenCaptureSavePath = Path.Combine(
                    App.LocalRPAStudioDir,
                    "ScreenRecorder",
                    obj.m_packageItem.Name,
                    "(",
                    DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒"),
                    ").mp4"
                );
                FFmpegService = new FFmpegService(screenCaptureSavePath, ViewModelLocator.Instance.UserPreferences.FPS, ViewModelLocator.Instance.UserPreferences.Quality);
                Task.Run(FFmpegService.StartCaptureScreen);

                // 三秒
                int num = 0;
                while (!FFmpegService.IsRunning())
                {
                    num++;
                    Thread.Sleep(300);
                    if (num == 10)
                    {
                        break;
                    }
                }
            }

            Common.RunInUI(() =>
            {
                ViewModelLocator.Instance.Main.m_view.Hide();

                obj.m_packageItem.IsRunning = true;

                IsWorkflowRunning = true;
                WorkflowRunningName = obj.m_packageItem.Name;
                WorkflowRunningToolTip = obj.m_packageItem.ToolTip;
                WorkflowRunningStatus = "正在运行";
            });
        }

        private void EndRun(RunManager obj)
        {
            SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "流程运行结束", null);
            //Task.Run(async ()=>
            //{
            //    if (obj.HasException)
            //    {
            //        await ControlServerService.UpdateRunStatus(obj.m_packageItem.Name, obj.m_packageItem.Version, ControlServerService.enProcessStatus.Exception);
            //    }
            //    else
            //    {
            //        await this.ControlServerService.UpdateRunStatus(obj.m_packageItem.Name, obj.m_packageItem.Version, ControlServerService.enProcessStatus.Stop);
            //    }
            //});
            if (ViewModelLocator.Instance.UserPreferences.IsEnableScreenRecorder)
            {
                SharedObject.Instance.Output(SharedObject.enOutputType.Trace, "屏幕录像结束", null);
                FFmpegService.StopCaptureScreen();
                FFmpegService = null;
            }
            Common.RunInUI(() =>
            {
                ViewModelLocator.Instance.Main.m_view.Show();
                ViewModelLocator.Instance.Main.m_view.Activate();

                obj.m_packageItem.IsRunning = false;

                //由于有可能列表已经刷新，所以需要重置IsRunning状态，为了方便，全部重置
                foreach (var pkg in PackageItems)
                {
                    pkg.IsRunning = false;
                }

                IsWorkflowRunning = false;
                WorkflowRunningName = "";
                WorkflowRunningStatus = "";
            });
        }
    }
}
