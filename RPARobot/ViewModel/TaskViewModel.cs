using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Mapster;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl.Matchers;
using RPARobot.Jobs;
using RPARobot.Librarys;
using RPARobot.Models;
using RPARobot.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RPARobot.ViewModel
{
    public class TaskViewModel : ViewModelBase
    {
        public TaskViewModel()
        {

        }

        private ObservableCollection<ScheduledTaskInfo> _tasks = new ObservableCollection<ScheduledTaskInfo>();
        public ObservableCollection<ScheduledTaskInfo> Tasks
        {
            get => _tasks;

            set
            {
                if (value != _tasks)
                {
                    _tasks = value;
                    RaisePropertyChanged(nameof(Tasks));
                }
            }
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(async () =>
        {
            await LoadTaskAsync();
        }));

        private RelayCommand _openForm;
        public RelayCommand OpenForm => _openForm ?? (_openForm = new RelayCommand(async () =>
        {
            var window = new TaskEditWindow();
            window.Owner = ViewModelLocator.Instance.Main.m_view;
            window.DataContext = ViewModelLocator.CreateTaskFormViewModel();
            var result = window.ShowDialog();
            // 添加成功
            if (result == true)
            {
                await LoadTaskAsync();
            }
        }));

        public async Task LoadTaskAsync()
        {
            Tasks.Clear();

            var jobKeys = await App.JobScheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            if (jobKeys != null)
            {
                foreach (var jobKey in jobKeys)
                {
                    var jobDetail = await App.JobScheduler.GetJobDetail(jobKey);
                    var data = jobDetail.JobDataMap.GetString(RunFlowJob.ParameterJobData);
                    if (!string.IsNullOrEmpty(data))
                    {
                        try
                        {
                            var taskDetial = JsonConvert.DeserializeObject<ScheduledTask>(data);

                            var taskInfo = taskDetial.Adapt<ScheduledTaskInfo>();
                            taskInfo.IsRunning = await QuartzHelper.JobIsRunning(jobDetail.Key);
                            taskInfo.NextOccurrence = CronHelper.GetNextValidTimeAfter(taskDetial.CronExpression)?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            Tasks.Add(taskInfo);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
    }
}
