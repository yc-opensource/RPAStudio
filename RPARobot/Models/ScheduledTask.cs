using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Models
{
    /// <summary>
    /// 计划任务
    /// </summary>
    public class ScheduledTask
    {
        public Guid Id { get; set; }
        public string TaskName { get; set; }
        public string PackageDir { get; set; }
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public string CronExpression { get; set; }
        public bool IsEnabled { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset ModificationTime { get; set; }
        public string JobName { get; set; }
        public int CreatingMode { get; set; }
        public int Cycle { get; internal set; }
    }
}
