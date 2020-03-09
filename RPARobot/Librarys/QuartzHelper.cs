using Microsoft.Data.Sqlite;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Librarys
{
    public static class QuartzHelper
    {
        public static async Task<IScheduler> InitSchedulerAsync()
        {
            DbProvider.RegisterDbMetadata("sqlite-custom", new DbMetadata()
            {
                AssemblyName = typeof(SqliteConnection).Assembly.GetName().Name,
                ConnectionType = typeof(SqliteConnection),
                CommandType = typeof(SqliteCommand),
                ParameterType = typeof(SqliteParameter),
                ParameterDbType = typeof(DbType),
                ParameterDbTypePropertyName = "DbType",
                ParameterNamePrefix = "@",
                ExceptionType = typeof(SqliteException),
                BindByName = true
            });


            var properties = new NameValueCollection
            {
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = "true",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz",
                ["quartz.dataSource.default.provider"] = "sqlite-custom",
                ["quartz.dataSource.default.connectionString"] = "Data Source=database.db",
                ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                ["quartz.serializer.type"] = "binary"
            };


            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler sched = await sf.GetScheduler();
            return sched;
        }

        public static async Task<bool> JobIsRunning(JobKey jobKey)
        {
            var currentlyExecutingJobs = await App.JobScheduler.GetCurrentlyExecutingJobs();
            if (currentlyExecutingJobs != null)
            {
                return currentlyExecutingJobs.Any(job => job.JobDetail.Key.Equals(jobKey));
            }
            return false;
        }
    }
}
