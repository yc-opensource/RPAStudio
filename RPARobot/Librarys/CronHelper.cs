using Quartz;
using RPARobot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Librarys
{
    /// <summary>
    /// 周为 1-7 1为星期天
    /// </summary>
    public static class CronHelper
    {
        public static bool IsValidExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }

        public static string BuildCronExpression(TaskScheduleModel taskScheduleModel)
        {
            // 一次性的
            if (taskScheduleModel.Cycle == Cycle.Disposable.Key)
            {
                return string.Empty;
            }

            var cronExpression = new StringBuilder();

            // 秒
            cronExpression.Append(GetSMHValue(taskScheduleModel.Second));
            //分  
            cronExpression.Append(GetSMHValue(taskScheduleModel.Minute));
            //小时  
            cronExpression.Append(GetSMHValue(taskScheduleModel.Hour));

            // 每天
            if (taskScheduleModel.Cycle == Cycle.Daily.Key)
            {
                // 日
                cronExpression.Append("* ");
                // 月
                cronExpression.Append("* ");
                // 周
                cronExpression.Append("?");
            }
            // 每周 
            else if (taskScheduleModel.Cycle == Cycle.Weekly.Key)
            {
                // 一个月中第几天 
                cronExpression.Append("? ");
                // 月份  
                cronExpression.Append("* ");
                // 周 
                if (string.IsNullOrWhiteSpace(taskScheduleModel.DayOfWeeks))
                {
                    throw new Exception("请设置周");
                }
                // 一周的哪几天
                cronExpression.Append(taskScheduleModel.DayOfWeeks);
            }
            // 每月
            else if (taskScheduleModel.Cycle == Cycle.Monthly.Key)
            {
                if (string.IsNullOrWhiteSpace(taskScheduleModel.DayOfMonths))
                {
                    throw new Exception("请设置日");
                }
                // 一个月的哪几天
                cronExpression.Append(taskScheduleModel.DayOfMonths);

                // 月份
                cronExpression.Append(" * ");
                // 周
                cronExpression.Append("?");
            }

            var cron = cronExpression.ToString();
            return cron;
        }

        /// <summary>
        /// 获取时分秒值
        /// </summary>
        /// <returns></returns>
        private static string GetSMHValue(int? value)
        {
            if (value >= 0)
            {
                return $"{value} ";
            }
            else
            {
                return "* ";
            }
        }

        public static DateTimeOffset? GetNextValidTimeAfter(string cronExpression)
        {
            var cron = new CronExpression(cronExpression);
            return cron.GetNextValidTimeAfter(DateTime.UtcNow);
        }
    }
}
