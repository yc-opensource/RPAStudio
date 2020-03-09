using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;
using Quartz;
using RPARobot.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Jobs
{
    /// <summary>
    /// 移除过期的任务
    /// </summary>
    public class RemoveExpiredTaskJob : IJob
    {
        public const string RemovedJobNameParameterName = "RemovedJobName";

        public async Task Execute(IJobExecutionContext context)
        {
            var map = context.JobDetail.JobDataMap;
            var jobName = map.GetString(RemovedJobNameParameterName);
            if (!string.IsNullOrEmpty(jobName))
            {
                var jobKey = new JobKey(jobName);
                var result = await App.JobScheduler.DeleteJob(jobKey);
                // TODO 记录日志
                if (result)
                {
                    Console.WriteLine($"已删除任务：{jobName}");
                    // 刷新任务列表
                    var navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
                    if (navigationService.CurrentPageKey == nameof(ViewModelLocator.Task))
                    {
                        await ViewModelLocator.Instance.Task.LoadTaskAsync();
                    }
                }
                Console.WriteLine("删除失败");
            }
        }
    }
}
