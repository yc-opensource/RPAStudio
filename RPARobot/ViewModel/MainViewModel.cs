using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using NuGet;
using System.Linq;
using RPARobot.Librarys;
using System.Collections.ObjectModel;
using log4net;
using System.Collections.Generic;
using RPARobot.Executor;
using GalaSoft.MvvmLight.Messaging;
using RPARobot.Navigation;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;

namespace RPARobot.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RunManager m_runManager { get; set; }

        public Window m_view { get; set; }

        public List<Navigation.MenuItem> Menus { get; }

        public string CurrentPageKey
        {
            get => _navigationService.CurrentPageKey;
            set
            {
                if (value != _navigationService.CurrentPageKey)
                {
                    _navigationService.NavigateTo(value);
                    RaisePropertyChanged(nameof(CurrentPageKey));
                }
            }
        }

        private INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navigationService)
        {
            Menus = new List<Navigation.MenuItem>
            {
                new MenuItem(nameof(ViewModelLocator.Flow), Plugins.Shared.Library.Controls.Icon.Flow, "我的流程"),
                new MenuItem(nameof(ViewModelLocator.Task), Plugins.Shared.Library.Controls.Icon.Schedule, "计划任务"),
                new MenuItem(nameof(ViewModelLocator.Log), Plugins.Shared.Library.Controls.Icon.Record, "运行记录"),
                new MenuItem(nameof(ViewModelLocator.UserPreferences), Plugins.Shared.Library.Controls.Icon.Settings, "用户设置")
            };

            _navigationService = navigationService;
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
                        m_view = (Window)p.Source;

                        //_navigationService.NavigateTo(nameof(ViewModelLocator.Flow));
                        CurrentPageKey = nameof(ViewModelLocator.Flow);
                    }));
            }
        }

        private RelayCommand _MouseLeftButtonDownCommand;

        /// <summary>
        /// Gets the MouseLeftButtonDownCommand.
        /// </summary>
        public RelayCommand MouseLeftButtonDownCommand
        {
            get
            {
                return _MouseLeftButtonDownCommand
                    ?? (_MouseLeftButtonDownCommand = new RelayCommand(
                    () =>
                    {
                        //点标题外的部分也能拖动，方便使用
                        m_view.DragMove();
                    }));
            }
        }

        private RelayCommand _activatedCommand;

        /// <summary>
        /// Gets the ActivatedCommand.
        /// </summary>
        public RelayCommand ActivatedCommand
        {
            get
            {
                return _activatedCommand
                    ?? (_activatedCommand = new RelayCommand(
                    () =>
                    {
                        ViewModelLocator.Instance.Flow.RefreshAllPackages();
                    }));
            }
        }



        private RelayCommand<System.ComponentModel.CancelEventArgs> _closingCommand;

        /// <summary>
        /// Gets the ClosingCommand.
        /// </summary>
        public RelayCommand<System.ComponentModel.CancelEventArgs> ClosingCommand
        {
            get
            {
                return _closingCommand
                    ?? (_closingCommand = new RelayCommand<System.ComponentModel.CancelEventArgs>(
                    e =>
                    {
                        e.Cancel = true;//不关闭窗口
                        m_view.Hide();
                    }));
            }
        }


        private RelayCommand _userPreferencesCommand;

        /// <summary>
        /// Gets the UserPreferencesCommand.
        /// </summary>
        public RelayCommand UserPreferencesCommand
        {
            get
            {
                return _userPreferencesCommand
                    ?? (_userPreferencesCommand = new RelayCommand(
                    () =>
                    {
                        if (!ViewModelLocator.Instance.Startup.UserPreferencesWindow.IsVisible)
                        {
                            var vm = ViewModelLocator.Instance.Startup.UserPreferencesWindow.DataContext as UserPreferencesViewModel;
                            vm.LoadSettings();

                            ViewModelLocator.Instance.Startup.UserPreferencesWindow.Show();
                        }

                        ViewModelLocator.Instance.Startup.UserPreferencesWindow.Activate();
                    }));
            }
        }


        private RelayCommand _viewLogsCommand;

        /// <summary>
        /// Gets the ViewLogsCommand.
        /// </summary>
        public RelayCommand ViewLogsCommand
        {
            get
            {
                return _viewLogsCommand
                    ?? (_viewLogsCommand = new RelayCommand(
                    () =>
                    {
                        //打开日志所在的目录
                        Common.LocateDirInExplorer(App.LocalRPAStudioDir + @"\Logs");
                    }));
            }
        }


        private RelayCommand _registerProductCommand;

        /// <summary>
        /// Gets the RegisterProductCommand.
        /// </summary>
        public RelayCommand RegisterProductCommand
        {
            get
            {
                return _registerProductCommand
                    ?? (_registerProductCommand = new RelayCommand(
                    () =>
                    {
                        //弹出注册窗口
                        if (!ViewModelLocator.Instance.Startup.RegisterWindow.IsVisible)
                        {
                            var vm = ViewModelLocator.Instance.Startup.RegisterWindow.DataContext as RegisterViewModel;
                            vm.LoadRegisterInfo();

                            ViewModelLocator.Instance.Startup.RegisterWindow.Show();
                        }

                        ViewModelLocator.Instance.Startup.RegisterWindow.Activate();
                    }));
            }
        }





        /// <summary>
        /// The <see cref="WorkflowRunningStatus" /> property's name.
        /// </summary>
        public const string WorkflowRunningStatusPropertyName = "WorkflowRunningStatus";

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
                RaisePropertyChanged(WorkflowRunningStatusPropertyName);
            }
        }




        
    }
}