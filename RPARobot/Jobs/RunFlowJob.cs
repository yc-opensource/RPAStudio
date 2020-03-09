using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet;
using Plugins.Shared.Library;
using Quartz;
using RPARobot.Executor;
using RPARobot.Models;
using RPARobot.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Jobs
{
    public class RunFlowJob : IJob
    {
        public const string ParameterJobData = "ScheduledTaskData";

        public async Task Execute(IJobExecutionContext context)
        {
            var map = context.JobDetail.JobDataMap;
            var scheduledTaskData = map.GetString(ParameterJobData);
            if (!string.IsNullOrEmpty(scheduledTaskData))
            {
                var task = JsonConvert.DeserializeObject<ScheduledTask>(scheduledTaskData);

                Console.WriteLine($"执行任务：{task.TaskName}");

                var packageItem = GetPackageItem(task.PackageName);

                //如果已经有一个项目正在运行，则不允许再运行
                if (ViewModelLocator.Instance.Flow.IsWorkflowRunning)
                {
                    Console.WriteLine("已经有工作流正在运行，请等待它结束后再运行！");
                    return;
                }

                var projectDir = ViewModelLocator.Instance.Flow.InstalledPackagesDir + @"\" + task.PackageName + @"." + packageItem.Version + @"\lib\net452";
                var projectJsonFile = projectDir + @"\project.json";
                if (System.IO.File.Exists(projectJsonFile))
                {
                    //项目配置文件存在
                    //1.找到主XAML文件，然后运行它
                    string json = System.IO.File.ReadAllText(projectJsonFile);
                    JObject jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                    var relativeMainXaml = jsonObj["main"].ToString();
                    var absoluteMainXaml = System.IO.Path.Combine(projectDir, relativeMainXaml);

                    if (System.IO.File.Exists(absoluteMainXaml))
                    {
                        RunWorkflow(packageItem, projectDir, absoluteMainXaml);
                    }
                }
            }
        }

        private PackageItem GetPackageItem(string packageName)
        {
            var commonApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
            var packagesDir = commonApplicationData + @"\RPAStudio\Packages";//机器人默认读取nupkg包的位置
            var installedPackagesDir = commonApplicationData + @"\RPAStudio\InstalledPackages";//机器人默认读取nupkg包的位置
            var repo = PackageRepositoryFactory.Default.CreateRepository(packagesDir);

            Dictionary<string, IPackage> installedPkgDict = new Dictionary<string, IPackage>();

            var packageManager = new PackageManager(repo, installedPackagesDir);
            foreach (IPackage pkg in packageManager.LocalRepository.GetPackages())
            {
                installedPkgDict[pkg.Id] = pkg;
            }


            var item = new PackageItem();
            item.Name = packageName;

            var version = repo.FindPackagesById(packageName).Max(p => p.Version);
            item.Version = version.ToString();

            var pkgNameList = repo.FindPackagesById(packageName);
            foreach (var i in pkgNameList)
            {
                item.VersionList.Add(i.Version.ToString());
            }

            bool isNeedUpdate = false;
            if (installedPkgDict.ContainsKey(item.Name))
            {
                var installedVer = installedPkgDict[item.Name].Version;
                if (version > installedVer)
                {
                    isNeedUpdate = true;
                }
            }
            else
            {
                isNeedUpdate = true;
            }
            item.IsNeedUpdate = isNeedUpdate;

            var package = repo.FindPackage(packageName, version);
            item.Package = package;
            var publishedTime = package.Published.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            item.ToolTip = string.Format("名称：{0}\r\n版本：{1}\r\n发布说明：{2}\r\n项目描述：{3}\r\n发布时间：{4}", item.Name, item.Version, package.ReleaseNotes, package.Description, (publishedTime == null ? "未知" : publishedTime));
            return item;
        }

        private void RunWorkflow(PackageItem packageItem, string projectDir, string absoluteMainXaml)
        {
            System.GC.Collect();//提醒系统回收内存，避免内存占用过高

            SharedObject.Instance.ProjectPath = projectDir;
            SharedObject.Instance.SetOutputFun(LogToOutputWindow);

            ViewModelLocator.Instance.Main.m_runManager = new RunManager(packageItem, absoluteMainXaml);
            ViewModelLocator.Instance.Main.m_runManager.Run();
        }

        private void LogToOutputWindow(SharedObject.enOutputType type, string msg, string msgDetails)
        {
            Librarys.Logger.Info(string.Format("活动日志：type={0},msg={1},msgDetails={2}", type.ToString(), msg, msgDetails));
        }
    }
}
