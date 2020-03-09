using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Plugins.Shared.Library.Nuget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Services
{
	public class LoadDependenciesService
	{
		public string CurrentProjectJsonFile { get; }

		public LoadDependenciesService(string projectJsonFile)
		{
			this.CurrentProjectJsonFile = projectJsonFile;
		}

		public async Task LoadDependencies()
		{
			ProjectJsonConfig projectJsonConfig = this.ProcessProjectJsonConfig();
			foreach (JToken jtoken in ((IEnumerable<JToken>)projectJsonConfig.dependencies))
			{
				JProperty jproperty = (JProperty)jtoken;
				VersionRange versionRange = VersionRange.Parse((string)jproperty.Value);
				if (versionRange.IsMinInclusive)
				{
					NuGetVersion version = NuGetVersion.Parse(versionRange.MinVersion.ToString());
					await NuGetPackageController.Instance.DownloadAndInstall(new PackageIdentity(jproperty.Name, version));
				}
			}
			IEnumerator<JToken> enumerator = null;
			foreach (string text in Directory.GetFiles(NuGetPackageController.Instance.TargetFolder, "*.dll", SearchOption.TopDirectoryOnly))
			{
				string text2 = Environment.CurrentDirectory + "\\" + Path.GetFileName(text);
				if (File.Exists(text2))
				{
					Librarys.Logger.Debug(string.Format("发现主程序所在目录下存在同名dll，忽略加载 {0}", text2), logger);
				}
				else
				{
					Assembly.LoadFrom(text);
					Path.GetFileNameWithoutExtension(text);
				}
			}
		}

		public ProjectJsonConfig ProcessProjectJsonConfig()
		{
			string value = File.ReadAllText(this.CurrentProjectJsonFile);
			ProjectJsonConfig result;
			try
			{
				ProjectJsonConfig projectJsonConfig = JsonConvert.DeserializeObject<ProjectJsonConfig>(value);
				if (projectJsonConfig.Upgrade())
				{
					string contents = JsonConvert.SerializeObject(projectJsonConfig, Formatting.Indented);
					File.WriteAllText(this.CurrentProjectJsonFile, contents);
					value = File.ReadAllText(this.CurrentProjectJsonFile);
					projectJsonConfig = JsonConvert.DeserializeObject<ProjectJsonConfig>(value);
				}
				result = projectJsonConfig;
			}
			catch (Exception)
			{
				throw;
			}
			return result;
		}

		private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}
}
