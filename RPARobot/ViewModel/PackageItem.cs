using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RPARobot.Librarys;
using System.Windows;
using System.Windows.Controls;
using System;
using NuGet;
using Newtonsoft.Json.Linq;
using RPARobot.Executor;
using Plugins.Shared.Library;
using log4net;
using System.Collections.Generic;
using System.Threading.Tasks;
using RPARobot.Services;
using System.IO;
using Newtonsoft.Json;

namespace RPARobot.ViewModel
{
    public class PackageItem : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IPackage Package { get; internal set; }

        public List<string> VersionList { get; set; } = new List<string>();

        /// <summary>
        /// The <see cref="IsRunning" /> property's name.
        /// </summary>
        public const string IsRunningPropertyName = "IsRunning";

        private bool _isRunningProperty = false;

        /// <summary>
        /// Sets and gets the IsRunning property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _isRunningProperty;
            }

            set
            {
                if (_isRunningProperty == value)
                {
                    return;
                }

                _isRunningProperty = value;
                RaisePropertyChanged(IsRunningPropertyName);

                //更新按钮状态
                StartCommand.RaiseCanExecuteChanged();
                UpdateCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _nameProperty = "";

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _nameProperty;
            }

            set
            {
                if (_nameProperty == value)
                {
                    return;
                }

                _nameProperty = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Version" /> property's name.
        /// </summary>
        public const string VersionPropertyName = "Version";

        private string _versionProperty = "";

        /// <summary>
        /// Sets and gets the Version property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Version
        {
            get
            {
                return _versionProperty;
            }

            set
            {
                if (_versionProperty == value)
                {
                    return;
                }

                _versionProperty = value;
                RaisePropertyChanged(VersionPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsMouseOver" /> property's name.
        /// </summary>
        public const string IsMouseOverPropertyName = "IsMouseOver";

        private bool _isMouseOverProperty = false;

        /// <summary>
        /// Sets and gets the IsMouseOver property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return _isMouseOverProperty;
            }

            set
            {
                if (_isMouseOverProperty == value)
                {
                    return;
                }

                _isMouseOverProperty = value;
                RaisePropertyChanged(IsMouseOverPropertyName);
            }
        }



        private RelayCommand _mouseEnterCommand;

        /// <summary>
        /// Gets the MouseEnterCommand.
        /// </summary>
        public RelayCommand MouseEnterCommand
        {
            get
            {
                return _mouseEnterCommand
                    ?? (_mouseEnterCommand = new RelayCommand(
                    () =>
                    {
                        IsMouseOver = true;
                    }));
            }
        }


        private RelayCommand _mouseLeaveCommand;

        /// <summary>
        /// Gets the MouseLeaveCommand.
        /// </summary>
        public RelayCommand MouseLeaveCommand
        {
            get
            {
                return _mouseLeaveCommand
                    ?? (_mouseLeaveCommand = new RelayCommand(
                    () =>
                    {
                        IsMouseOver = false;
                    }));
            }
        }


        /// <summary>
        /// The <see cref="IsNeedUpdate" /> property's name.
        /// </summary>
        public const string IsNeedUpdatePropertyName = "IsNeedUpdate";

        private bool _isNeedUpdateProperty = false;

        /// <summary>
        /// Sets and gets the IsNeedUpdate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsNeedUpdate
        {
            get
            {
                return _isNeedUpdateProperty;
            }

            set
            {
                if (_isNeedUpdateProperty == value)
                {
                    return;
                }

                _isNeedUpdateProperty = value;
                RaisePropertyChanged(IsNeedUpdatePropertyName);
            }
        }


        private RelayCommand _copyItemInfoCommand;

        /// <summary>
        /// Gets the CopyItemInfoCommand.
        /// </summary>
        public RelayCommand CopyItemInfoCommand
        {
            get
            {
                return _copyItemInfoCommand
                    ?? (_copyItemInfoCommand = new RelayCommand(
                    () =>
                    {
                        Clipboard.SetDataObject(ToolTip);
                    }));
            }
        }


        private RelayCommand _locateItemCommand;

        /// <summary>
        /// Gets the LocateItemCommand.
        /// </summary>
        public RelayCommand LocateItemCommand
        {
            get
            {
                return _locateItemCommand
                    ?? (_locateItemCommand = new RelayCommand(
                    () =>
                    {
                        var file = ViewModelLocator.Instance.Flow.PackagesDir + @"\" + Name + @"." + Version + ".nupkg";
                        Common.LocateFileInExplorer(file);
                    }));
            }
        }



        void DeleteNuPkgsFile(bool bRefresh = true)
        {
            //删除nupkg安装包
            foreach (var ver in VersionList)
            {
                var file = ViewModelLocator.Instance.Flow.PackagesDir + @"\" + Name + @"." + ver + ".nupkg";
                Common.DeleteFile(file);
            }

            if (bRefresh)
            {
                Common.RunInUI(() =>
                {
                    //刷新
                    ViewModelLocator.Instance.Flow.RefreshCommand.Execute(null);
                });
            }

        }


        private RelayCommand _removeItemCommand;

        /// <summary>
        /// Gets the RemoveItemCommand.
        /// </summary>
        public RelayCommand RemoveItemCommand
        {
            get
            {
                return _removeItemCommand
                    ?? (_removeItemCommand = new RelayCommand(
                    () =>
                    {
                        var ret = MessageBox.Show(App.Current.MainWindow, "确定移除当前包吗？", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                        if (ret == MessageBoxResult.Yes)
                        {
                            //卸载已安装的包，删除nupkg包
                            if (this.Package != null)
                            {
                                var repo = PackageRepositoryFactory.Default.CreateRepository(ViewModelLocator.Instance.Flow.PackagesDir);
                                var packageManager = new PackageManager(repo, ViewModelLocator.Instance.Flow.InstalledPackagesDir);

                                packageManager.PackageUninstalled += (sender, eventArgs) =>
                                {
                                    //如果都卸载完了，才刷新
                                    if (!packageManager.LocalRepository.Exists(this.Name))
                                    {
                                        DeleteNuPkgsFile();
                                    }
                                };

                                if (packageManager.LocalRepository.Exists(this.Name))
                                {
                                    while (packageManager.LocalRepository.Exists(this.Name))
                                    {
                                        packageManager.UninstallPackage(this.Name);
                                    }

                                }
                                else
                                {
                                    DeleteNuPkgsFile();
                                }


                            }
                        }
                    },
                    () => !IsRunning));
            }
        }
        private RelayCommand _mouseRightButtonUpCommand;

        /// <summary>
        /// Gets the MouseRightButtonUpCommand.
        /// </summary>
        public RelayCommand MouseRightButtonUpCommand
        {
            get
            {
                return _mouseRightButtonUpCommand
                    ?? (_mouseRightButtonUpCommand = new RelayCommand(
                    () =>
                    {
                        var view = App.Current.MainWindow;
                        var cm = view.FindResource("PackageItemContextMenu") as ContextMenu;
                        cm.DataContext = this;
                        cm.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                        cm.IsOpen = true;
                    }));
            }
        }




        private RelayCommand _mouseDoubleClickCommand;

        /// <summary>
        /// Gets the MouseDoubleClickCommand.
        /// </summary>
        public RelayCommand MouseDoubleClickCommand
        {
            get
            {
                return _mouseDoubleClickCommand
                    ?? (_mouseDoubleClickCommand = new RelayCommand(
                    () =>
                    {
                        updateOrStart();
                    }));
            }
        }

        /// <summary>
        /// 如果未安装，则安装，如果需要更新则更新，如果能运行则运行，只执行一个步骤
        /// </summary>
        private void updateOrStart()
        {
            if (IsNeedUpdate)
            {
                UpdateCommand.Execute(null);
            }
            else
            {
                StartCommand.Execute(null);
            }
        }



        /// <summary>
        /// The <see cref="IsVisible" /> property's name.
        /// </summary>
        public const string IsVisiblePropertyName = "IsVisible";

        private bool _isVisibleProperty = true;

        /// <summary>
        /// Sets and gets the IsVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _isVisibleProperty;
            }

            set
            {
                if (_isVisibleProperty == value)
                {
                    return;
                }

                _isVisibleProperty = value;
                RaisePropertyChanged(IsVisiblePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="IsSelected" /> property's name.
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelectedProperty = false;

        /// <summary>
        /// Sets and gets the IsSelected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelectedProperty;
            }

            set
            {
                if (_isSelectedProperty == value)
                {
                    return;
                }

                _isSelectedProperty = value;
                RaisePropertyChanged(IsSelectedPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="ToolTip" /> property's name.
        /// </summary>
        public const string ToolTipPropertyName = "ToolTip";

        private string _toolTipProperty = null;

        /// <summary>
        /// Sets and gets the ToolTip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ToolTip
        {
            get
            {
                return _toolTipProperty;
            }

            set
            {
                if (_toolTipProperty == value)
                {
                    return;
                }

                _toolTipProperty = value;
                RaisePropertyChanged(ToolTipPropertyName);
            }
        }




        /// <summary>
        /// The <see cref="IsSearching" /> property's name.
        /// </summary>
        public const string IsSearchingPropertyName = "IsSearching";

        private bool _isSearchingProperty = false;

        /// <summary>
        /// Sets and gets the IsSearching property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSearching
        {
            get
            {
                return _isSearchingProperty;
            }

            set
            {
                if (_isSearchingProperty == value)
                {
                    return;
                }

                _isSearchingProperty = value;
                RaisePropertyChanged(IsSearchingPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SearchText" /> property's name.
        /// </summary>
        public const string SearchTextPropertyName = "SearchText";

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
                RaisePropertyChanged(SearchTextPropertyName);
            }
        }



        /// <summary>
        /// The <see cref="IsMatch" /> property's name.
        /// </summary>
        public const string IsMatchPropertyName = "IsMatch";

        private bool _isMatchProperty = false;

        /// <summary>
        /// Sets and gets the IsMatch property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsMatch
        {
            get
            {
                return _isMatchProperty;
            }

            set
            {
                if (_isMatchProperty == value)
                {
                    return;
                }

                _isMatchProperty = value;
                RaisePropertyChanged(IsMatchPropertyName);
            }
        }


        public void ApplyCriteria(string criteria)
        {
            SearchText = criteria;

            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;

            }
        }


        private bool IsCriteriaMatched(string criteria)
        {
            return string.IsNullOrEmpty(criteria) || Name.ContainsIgnoreCase(criteria);
        }








        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        private RelayCommand _startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand
                    ?? (_startCommand = new RelayCommand(
                    () =>
                    {
                        Log(SharedObject.enOutputType.Trace, "流程启动");
                        bool flag = ViewModelLocator.Instance.Register.IsNotExpired();
                        ViewModelLocator.Instance.Startup.RefreshProgramStatus(flag);
                        if (!flag)
                        {
                            this.Log(SharedObject.enOutputType.Warning, "软件未通过授权检测，请注册产品！");
                            AutoCloseMessageBoxService.Show(Application.Current.MainWindow, "软件未通过授权检测，请注册产品！", "提示", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.Yes);
                            return;
                        }
                        if (ViewModelLocator.Instance.Flow.IsWorkflowRunning)
                        {
                            this.Log(SharedObject.enOutputType.Warning, "已经有工作流正在运行，请等待它结束后再运行！");
                            AutoCloseMessageBoxService.Show(Application.Current.MainWindow, "已经有工作流正在运行，请等待它结束后再运行！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.Yes);
                            return;
                        }
                        string projectDir = string.Concat(new string[]
                        {
                            ViewModelLocator.Instance.Flow.InstalledPackagesDir,
                            "\\",
                            this.Name,
                            ".",
                            this.Version,
                            "\\lib\\net452"
                        });
                        string projectJsonFile = projectDir + "\\project.json";
                        if (File.Exists(projectJsonFile))
                        {
                            Task.Run(async delegate ()
                            {
                                try
                                {
                                    Common.RunInUI(delegate
                                    {
                                        IsRunning = true;
                                        ViewModelLocator.Instance.Flow.IsWorkflowRunning = true;
                                        ViewModelLocator.Instance.Flow.WorkflowRunningName = Name;
                                        ViewModelLocator.Instance.Flow.WorkflowRunningToolTip = ToolTip;
                                        ViewModelLocator.Instance.Flow.WorkflowRunningStatus = "正在加载项目依赖项";
                                    });
                                    await new LoadDependenciesService(projectJsonFile).LoadDependencies();
                                    string path = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(projectJsonFile))["main"].ToString();
                                    string text = Path.Combine(projectDir, path);
                                    if (File.Exists(text))
                                    {
                                        RunWorkflow(projectDir, text);
                                    }
                                }
                                catch (Exception message)
                                {
                                    Logger.Error(message, PackageItem.logger);
                                    Common.RunInUI(delegate
                                    {
                                        IsRunning = false;
                                        Log(SharedObject.enOutputType.Warning, "加载项目依赖项出错！");
                                        AutoCloseMessageBoxService.Show(Application.Current.MainWindow, "加载项目依赖项出错！", "提示", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.Yes);
                                    });
                                }
                            });
                        }
                    },
                    () => !IsRunning, false));
            }
        }

        private void RunWorkflow(string projectDir, string absoluteMainXaml)
        {
            System.GC.Collect();//提醒系统回收内存，避免内存占用过高

            SharedObject.Instance.ProjectPath = projectDir;
            SharedObject.Instance.SetOutputFun(LogToOutputWindow);

            ViewModelLocator.Instance.Main.m_runManager = new RunManager(this, absoluteMainXaml);
            ViewModelLocator.Instance.Main.m_runManager.Run();
        }


        private void LogToOutputWindow(SharedObject.enOutputType type, string msg, string msgDetails)
        {
            Logger.Info(string.Format("活动日志：type={0},msg={1},msgDetails={2}", type.ToString(), msg, msgDetails), logger);
        }


        private RelayCommand _updateCommand;

        /// <summary>
        /// Gets the UpdateCommand.
        /// </summary>
        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand
                    ?? (_updateCommand = new RelayCommand(
                    () =>
                    {
                        //安装或更新包
                        if (this.Package != null)
                        {
                            var repo = PackageRepositoryFactory.Default.CreateRepository(ViewModelLocator.Instance.Flow.PackagesDir);
                            var packageManager = new PackageManager(repo, ViewModelLocator.Instance.Flow.InstalledPackagesDir);

                            packageManager.PackageInstalled += (sender, eventArgs) =>
                            {
                                ViewModelLocator.Instance.Flow.RefreshCommand.Execute(null);
                            };

                            packageManager.InstallPackage(this.Package, false, true, true);
                        }
                    },
                    () => !IsRunning));
            }
        }

        private void Log(SharedObject.enOutputType type, string msg)
        {
            Task.Run(async delegate ()
            {
                if (msg.Length > 150)
                {
                    msg = msg.Substring(0, 150);
                }
                //await ViewModelLocator.Instance.Main.ControlServerService.Log(this.Name, this.Version, type.ToString(), msg);
            });
        }
    }
}