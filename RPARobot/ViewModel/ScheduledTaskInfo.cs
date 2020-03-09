using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Quartz;
using RPARobot.Jobs;
using RPARobot.Models;
using RPARobot.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RPARobot.ViewModel
{
    public class ScheduledTaskInfo : ViewModelBase
    {
        public Guid Id { get; set; }
        public string TaskName { get; set; }
        public string ModificationTime { get; set; }
        public bool IsRunning { get; set; }
        public string JobName { get; set; }
        public string NextOccurrence { get; set; }

        private RelayCommand _pauseCommand;
        public RelayCommand PauseCommand => _pauseCommand ?? (_pauseCommand = new RelayCommand(async () =>
        {
            var jobDetail = await App.JobScheduler.GetJobDetail(new JobKey(JobName));
            if (jobDetail != null)
            {
                await App.JobScheduler.PauseJob(jobDetail.Key);
            }
        }));

        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(async () =>
        {
            if (MessageBox.Show("确定要删除吗？", "小摩RPA机器人", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var jobDetail = await App.JobScheduler.GetJobDetail(new JobKey(JobName));
                if (jobDetail != null)
                {
                    await App.JobScheduler.DeleteJob(jobDetail.Key);
                }
                await ViewModelLocator.Instance.Task.LoadTaskAsync();
            }
        }));

        private RelayCommand _editCommand;
        public RelayCommand EditCommand => _editCommand ?? (_editCommand = new RelayCommand(async () =>
        {
            var jobDetail = await App.JobScheduler.GetJobDetail(new JobKey(JobName));
            if (jobDetail != null)
            {
                var jobData = jobDetail.JobDataMap.GetString(RunFlowJob.ParameterJobData);
                var task = JsonConvert.DeserializeObject<ScheduledTask>(jobData);

                var window = new TaskEditWindow();
                window.Owner = ViewModelLocator.Instance.Main.m_view;
                window.DataContext = ViewModelLocator.CreateTaskFormViewModel(task);
                window.ShowDialog();
            }
        }));
    }
}
