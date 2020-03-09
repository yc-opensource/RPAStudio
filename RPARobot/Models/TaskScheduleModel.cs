using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Models
{
    public class TaskScheduleModel
    {
        /// <summary>
        /// 周期
        /// </summary>
        public int Cycle { get; set; } = Models.Cycle.Disposable.Key;
        /// <summary>
        /// 一周的哪几天
        /// </summary>
        public string DayOfWeeks { get; set; }
        /// <summary>
        /// 一个月的哪几天
        /// </summary>
        public string DayOfMonths { get; set; }
        /// <summary>
        /// 秒
        /// </summary>
        public int? Second { get; set; }
        /// <summary>
        /// 分
        /// </summary>
        public int? Minute { get; set; }
        /// <summary>
        /// 时
        /// </summary>
        public int? Hour { get; set; }
    }
}
