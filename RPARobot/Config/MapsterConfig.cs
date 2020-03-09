using Mapster;
using RPARobot.Models;
using RPARobot.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Config
{
    public class MapsterConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<ScheduledTask, ScheduledTaskInfo>.NewConfig()
                .Map(s => s.TaskName, d => d.TaskName)
                .Map(s => s.Id, d => d.Id)
                .Map(s => s.ModificationTime, d => d.ModificationTime.ToString("yyyy-MM-dd HH:mm"));
        }
    }
}
