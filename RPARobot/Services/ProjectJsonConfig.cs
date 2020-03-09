using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Services
{
    public class ProjectJsonConfig
    {
        [JsonProperty(Order = 1)]
        public string schemaVersion { get; set; }
        [JsonProperty(Required = Required.Always, Order = 2)]
        public string studioVersion { get; set; }
        [JsonProperty(Required = Required.Always, Order = 3)]
        public string projectType { get; set; }
        [JsonProperty(Order = 4)]
        public string projectVersion { get; set; }
        [JsonProperty(Required = Required.Always, Order = 5)]
        public string name { get; set; }
        [JsonProperty(Order = 6)]
        public string description { get; set; }
        [JsonProperty(Required = Required.Always, Order = 7)]
        public string main { get; set; }
        public bool Upgrade()
        {
            string schemaVersion = this.schemaVersion;
            this.Upgrade(ref schemaVersion, ProjectJsonConfig.initial_schema_version);
            if (this.schemaVersion == schemaVersion)
            {
                return false;
            }
            this.schemaVersion = schemaVersion;
            return true;
        }
        private void Upgrade(ref string schemaVersion, string newSchemaVersion)
        {
            if (schemaVersion == newSchemaVersion)
            {
                return;
            }
            Version v = new Version(schemaVersion);
            new Version(newSchemaVersion);
            if (v < new Version("2.0.0.0"))
            {
                string text = "当前程序不再支持V2.0版本以下的旧版本项目！";
                AutoCloseMessageBoxService.Show(text);
                throw new Exception(text);
            }
            this.Upgrade(ref schemaVersion, ProjectJsonConfig.initial_schema_version);
        }
        public void Init()
        {
            this.schemaVersion = ProjectJsonConfig.initial_schema_version;
            this.projectVersion = ProjectJsonConfig.initial_project_version;
        }
        private static readonly string initial_schema_version = "2.0.0";
        private static readonly string initial_project_version = "2.0.0";
        [JsonProperty(Order = 8)]
        public JObject dependencies = new JObject();
    }
}
