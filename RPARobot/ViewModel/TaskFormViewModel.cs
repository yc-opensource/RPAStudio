using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using NuGet;
using Quartz;
using RPARobot.Jobs;
using RPARobot.Librarys;
using RPARobot.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RPARobot.ViewModel
{
    public class TaskFormViewModel : ViewModelBase
    {
        /// <summary>
        /// 是否是编辑模式
        /// </summary>
        private bool _isEdit = false;
        public TaskFormViewModel(ScheduledTask task = null)
        {
            if (task != null)
            {
                _isEdit = true;

                _taskId = task.Id;
                _taskName = task.TaskName;
                _packageName = task.PackageName;
                _creatingMode = (CreatingMode)task.CreatingMode;
                _cycle = task.Cycle;
                // TODO 解析cron表达式
            }

            CreateDays();
        }

        public Window Window;

        /// <summary>
        /// 周期列表
        /// </summary>
        public List<Cycle> Cycles => Models.Cycle.Cycles;

        private RelayCommand<RoutedEventArgs> _loadedCommand;
        public RelayCommand<RoutedEventArgs> LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand<RoutedEventArgs>(args =>
        {
            Window = (Window)args.Source;

            var repo = PackageRepositoryFactory.Default.CreateRepository(ViewModelLocator.Instance.Flow.PackagesDir);
            var pkgList = repo.GetPackages();

            var pkgSet = new SortedSet<string>();
            foreach (var pkg in pkgList)
            {
                //通过set去重
                pkgSet.Add(pkg.Id);
            }
            foreach (var name in pkgSet)
            {
                var packages = repo.FindPackagesById(name);
                if (packages != null)
                {
                    var item = new Workflow();
                    item.Name = name;
                    item.PackagesDir = ViewModelLocator.Instance.Flow.PackagesDir;

                    packages.ToList().ForEach(p => item.Versions.Add(p.Version.ToString()));

                    Flows.Add(item);
                }
            }

        }));

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save, CanSave));

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(() =>
        {
            Window.DialogResult = false;
        }));

        #region ValueChanged Event Handler
        private RelayCommand<RoutedPropertyChangedEventArgs<object>> _executionTimeChangedCommand;
        public RelayCommand<RoutedPropertyChangedEventArgs<object>> ExecutionTimeChangedCommand =>
            _executionTimeChangedCommand ?? (_executionTimeChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>((args) =>
            {
                try
                {
                    var time = (TimeSpan)args.NewValue;
                    ExecutionTimeSpan = time;
                }
                catch (Exception)
                {
                }
            }));

        private RelayCommand<RoutedPropertyChangedEventArgs<object>> _startTimeChangedCommand;
        public RelayCommand<RoutedPropertyChangedEventArgs<object>> StartTimeChangedCommand
            => _startTimeChangedCommand ?? (_startTimeChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(args =>
            {
                if (DateTimeOffset.TryParse(args.NewValue?.ToString(), out var result))
                {
                    StartUtcTime = result;
                }
            }));

        private RelayCommand<RoutedPropertyChangedEventArgs<object>> _effectiveTimeChangedCommand;
        public RelayCommand<RoutedPropertyChangedEventArgs<object>> EffectiveTimeChangedCommand
            => _effectiveTimeChangedCommand ?? (_effectiveTimeChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(args =>
            {
                if (DateTimeOffset.TryParse(args.NewValue?.ToString(), out var result))
                {
                    EffectiveUtcTime = result;
                }
            }));
        #endregion

        /// <summary>
        /// 流程列表
        /// </summary>
        private ObservableCollection<Workflow> _flows = new ObservableCollection<Workflow>();
        public ObservableCollection<Workflow> Flows
        {
            get => _flows;
            set
            {
                if (value != _flows)
                {
                    _flows = value;
                    RaisePropertyChanged(nameof(Flows));
                }
            }
        }

        /// <summary>
        /// 创建模式
        /// </summary>
        private CreatingMode _creatingMode = CreatingMode.Quick;
        public CreatingMode CreatingMode
        {
            get => _creatingMode;
            set
            {
                if (value != _creatingMode)
                {
                    _creatingMode = value;
                    RaisePropertyChanged(nameof(CreatingMode));
                }
            }
        }

        /// <summary>
        /// 任务ID
        /// </summary>
        private Guid _taskId = Guid.NewGuid();
        public Guid TaskId
        {
            get => _taskId;
            set
            {
                if (value != _taskId)
                {
                    _taskId = value;
                    RaisePropertyChanged(nameof(TaskId));
                }
            }
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        private string _taskName;
        public string TaskName
        {
            get => _taskName;
            set
            {
                if (value != _taskName)
                {
                    _taskName = value;
                    RaisePropertyChanged(nameof(TaskName));
                }
            }
        }

        /// <summary>
        /// 选择的流程
        /// </summary>
        private string _packageName;
        public string PackageName
        {
            get => _packageName;
            set
            {
                if (value != _packageName)
                {
                    _packageName = value;
                    RaisePropertyChanged(nameof(PackageName));
                }
            }
        }

        #region Cron表达式
        private string _cronSecond = "*";
        public string CronSecond
        {
            get => _cronSecond;
            set
            {
                if (value != _cronSecond)
                {
                    _cronSecond = value;
                    RaisePropertyChanged(nameof(CronSecond));
                }
            }
        }
        private string _cronMinute = "*";
        public string CronMinute
        {
            get => _cronMinute;
            set
            {
                if (value != _cronMinute)
                {
                    _cronMinute = value;
                    RaisePropertyChanged(nameof(CronMinute));
                }
            }
        }
        private string _cronHour = "*";
        public string CronHour
        {
            get => _cronHour;
            set
            {
                if (value != _cronHour)
                {
                    _cronHour = value;
                    RaisePropertyChanged(nameof(CronHour));
                }
            }
        }
        private string _cronDayOfMonth = "*";
        public string CronDayOfMonth
        {
            get => _cronDayOfMonth;
            set
            {
                if (value != _cronDayOfMonth)
                {
                    _cronDayOfMonth = value;
                    RaisePropertyChanged(nameof(CronDayOfMonth));
                }
            }
        }
        private string _cronMonth = "*";
        public string CronMonth
        {
            get => _cronMonth;
            set
            {
                if (value != _cronMonth)
                {
                    _cronMonth = value;
                    RaisePropertyChanged(nameof(CronMonth));
                }
            }
        }
        private string _cronDayOfWeek = "?";
        public string CronDayOfWeek
        {
            get => _cronDayOfWeek;
            set
            {
                if (value != _cronDayOfWeek)
                {
                    _cronDayOfWeek = value;
                    RaisePropertyChanged(nameof(CronDayOfWeek));
                }
            }
        }
        #endregion

        /// <summary>
        /// 星期几或者几号复选框列表
        /// </summary>
        private ObservableCollection<CheckBoxProps> _checkBoxes;
        public ObservableCollection<CheckBoxProps> CheckBoxes
        {
            get => _checkBoxes;
            set
            {
                if (value != _checkBoxes)
                {
                    _checkBoxes = value;
                    RaisePropertyChanged(nameof(CheckBoxes));
                }
            }
        }

        /// <summary>
        /// 选中的周期
        /// </summary>
        private int _cycle = Models.Cycle.Disposable.Key;
        public int Cycle
        {
            get => _cycle;
            set
            {
                if (value != _cycle)
                {
                    _cycle = value;
                    CreateDays();
                    RaisePropertyChanged(nameof(Cycle));
                }
            }
        }

        /// <summary>
        /// 周期性任务可用
        /// </summary>
        private TimeSpan _executionTimeSpan;
        public TimeSpan ExecutionTimeSpan
        {
            get => _executionTimeSpan;
            set
            {
                if (value != _executionTimeSpan)
                {
                    _executionTimeSpan = value;
                    RaisePropertyChanged(nameof(ExecutionTimeSpan));
                }
            }
        }

        /// <summary>
        /// 一次性任务可用
        /// </summary>
        private DateTimeOffset _startUtcTime;
        public DateTimeOffset StartUtcTime
        {
            get => _startUtcTime;
            set
            {
                if (value != _startUtcTime)
                {
                    _startUtcTime = value;
                    RaisePropertyChanged(nameof(StartUtcTime));
                }
            }
        }

        /// <summary>
        /// 任务有效期
        /// </summary>
        private DateTimeOffset _effectiveUtcTime;
        public DateTimeOffset EffectiveUtcTime
        {
            get => _effectiveUtcTime;
            set
            {
                if (value != _effectiveUtcTime)
                {
                    _effectiveUtcTime = value;
                    RaisePropertyChanged(nameof(EffectiveUtcTime));
                }
            }
        }

        /// <summary>
        /// 固定时间或者永久
        /// </summary>
        private EffectiveTime _effectiveTimeType = EffectiveTime.Forever;
        public EffectiveTime EffectiveTimeType
        {
            get => _effectiveTimeType;
            set
            {
                if (value != _effectiveTimeType)
                {
                    _effectiveTimeType = value;
                    RaisePropertyChanged(nameof(EffectiveTimeType));
                }
            }
        }

        private void CreateDays()
        {
            var result = new ObservableCollection<CheckBoxProps>();
            var days = 0;
            if (Cycle == Models.Cycle.Monthly.Key) days = 31;
            else if (Cycle == Models.Cycle.Weekly.Key) days = 7;
            for (var i = 1; i <= days; i++)
            {
                result.Add(new CheckBoxProps(i.ToString("00"), i));
            }
            CheckBoxes = result;
        }

        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        private ITrigger CreateTrigger(string cronExpression)
        {
            var triggerBuilder = TriggerBuilder.Create()
                   .WithIdentity($"trigger_{TaskId}".ToString());
            if (string.IsNullOrEmpty(cronExpression))
            {
                triggerBuilder.StartAt(StartUtcTime);
            }
            else
            {
                triggerBuilder.WithCronSchedule(cronExpression, x => x.InTimeZone(TimeZoneInfo.Utc));
            }
            return triggerBuilder.Build();
        }

        /// <summary>
        /// 构建cron表达式
        /// </summary>
        /// <returns></returns>
        private string BuildCronExpression()
        {
            // 快捷创建
            if (CreatingMode == CreatingMode.Quick)
            {
                var taskScheduleModel = new TaskScheduleModel();

                taskScheduleModel.Cycle = Cycle;

                taskScheduleModel.Second = ExecutionTimeSpan.Seconds;
                taskScheduleModel.Minute = ExecutionTimeSpan.Minutes;
                taskScheduleModel.Hour = ExecutionTimeSpan.Hours;

                var days = CheckBoxes.Where(c => c.IsChecked).Select(c => c.Value);
                var daysStr = days.Count() > 0 ? string.Join(",", days) : "*";
                // 每周几
                if (Cycle == Models.Cycle.Weekly.Key)
                {
                    taskScheduleModel.DayOfWeeks = daysStr;
                }
                // 每月几号
                else if (Cycle == Models.Cycle.Monthly.Key)
                {
                    taskScheduleModel.DayOfMonths = daysStr;
                }

                return CronHelper.BuildCronExpression(taskScheduleModel);
            }
            // 自定义cron表达式
            else
            {
                var cronExpression = $"{CronSecond} {CronMinute} {CronHour} {CronDayOfMonth} {CronMonth} {CronDayOfWeek}";
                if (!CronHelper.IsValidExpression(cronExpression))
                {
                    throw new Exception("不合法的cron表达式");
                }
                return cronExpression;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
            // 任务标识
            var jobName = $"job_{TaskId}";
            // 生成cron表达式
            var cronExpression = BuildCronExpression();
            // 新建任务
            if (_isEdit == false)
            {
                var task = new ScheduledTask
                {
                    Id = TaskId,
                    CreationTime = DateTimeOffset.UtcNow,
                    CreatingMode = (int)CreatingMode,
                    CronExpression = cronExpression,
                    IsEnabled = true,
                    PackageName = PackageName,
                    TaskName = TaskName,
                    PackageDir = ViewModelLocator.Instance.Flow.PackagesDir,
                    ModificationTime = DateTimeOffset.UtcNow,
                    JobName = jobName,
                    Cycle = Cycle
                };

                CreateWatcherJob(jobName);

                var trigger = CreateTrigger(cronExpression);
                var job = JobBuilder.Create<RunFlowJob>()
                    .WithIdentity(jobName)
                    .UsingJobData(RunFlowJob.ParameterJobData, JsonConvert.SerializeObject(task))
                    .Build();
                App.JobScheduler.ScheduleJob(job, trigger);
            }

            Window.DialogResult = true;
        }
        private bool CanSave()
        {
            // 任务名不能为空
            if (string.IsNullOrWhiteSpace(TaskName))
            {
                return false;
            }

            // 需要选择流程
            if (string.IsNullOrWhiteSpace(PackageName))
            {
                return false;
            }

            // 固定的有效期必须大于当前时间
            if (EffectiveTimeType == EffectiveTime.DateTime)
            {
                if (EffectiveUtcTime <= DateTimeOffset.MinValue ||
                    EffectiveUtcTime < DateTimeOffset.UtcNow)
                {
                    return false;
                }
            }

            if (CreatingMode == CreatingMode.Quick)
            {
                // 一次性的任务需要设置开始日期和时间
                if (Cycle == Models.Cycle.Disposable.Key)
                {
                    if (StartUtcTime <= DateTimeOffset.MinValue ||
                        StartUtcTime < DateTimeOffset.UtcNow)
                    {
                        return false;
                    }
                }
            }
            else
            {
                // 自定义的cron必须符合格式
                var cronExpression = $"{CronSecond} {CronMinute} {CronHour} {CronDayOfMonth} {CronMonth} {CronDayOfWeek}";
                if (!CronHelper.IsValidExpression(cronExpression))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 创建监控任务，监控任务有效期已过
        /// </summary>
        private void CreateWatcherJob(string jobName)
        {
            if (EffectiveTimeType == EffectiveTime.DateTime)
            {
                var id = Guid.NewGuid().ToString();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"trigger_{id}")
                    .StartAt(EffectiveUtcTime)
                    .Build();
                var job = JobBuilder.Create<RemoveExpiredTaskJob>()
                    .WithIdentity($"job_{id}")
                    .UsingJobData(RemoveExpiredTaskJob.RemovedJobNameParameterName, jobName)
                    .Build();
                App.JobScheduler.ScheduleJob(job, trigger);
            }
        }
    }

    public enum EffectiveTime
    {
        Forever,
        DateTime
    }

    public enum CreatingMode
    {
        Quick,
        Custom
    }

    public class Workflow
    {
        public string Name { get; set; }
        public List<string> Versions { get; set; }
        public string ToolTip { get; set; }
        public string PackagesDir { get; set; }
        public string SelectedVersion { get; set; }

        public Workflow()
        {
            Versions = new List<string>();
        }
    }

    public class CheckBoxProps : ViewModelBase
    {
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (value != _text)
                {
                    _text = value;
                    RaisePropertyChanged(nameof(Text));
                }
            }
        }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    RaisePropertyChanged(nameof(Value));
                }
            }
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    RaisePropertyChanged(nameof(IsChecked));
                }
            }
        }

        public CheckBoxProps(string text, int value)
        {
            _text = text;
            _value = value;
        }
    }
}
