using log4net;
using Newtonsoft.Json.Linq;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Content;

namespace RPAStudio.Services
{
    public class PublishService
    {
        private string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<bool> Publish(PackageBuilder builder, string nupkgLocation, string projectName, string publishVersion, string description)
        {
            bool result;
            try
            {
                var url = nupkgLocation + "/OSPServer/rest/dict/savedict";
                var upload_affix = nupkgLocation + "/OSPServer/rest/affix/dict/save";
                var guid = NewGuid();
                var jObj = new JObject();
                jObj["DCT_ID"] = "PROCESS_INFO";
                var jarray = new JArray();
                var jobject = new JObject();
                jobject["F_GUID"] = guid;
                jobject["PROCESSNAME"] = projectName;
                jobject["PROCESSVERSION"] = publishVersion;
                jobject["PROCESSDESCRIBE"] = description;
                jarray.Add(jobject);
                jObj["InsertPool"] = jarray;
                if ((int)(await url.PostJsonAsync(jObj).ReceiveJson<JObject>())["code"] != 1)
                {
                    Librarys.Logger.Debug("增加流程信息失败！" + jObj.ToString(), PublishService.logger);
                    result = false;
                }
                else
                {
                    string text = projectName + "." + publishVersion + ".nupkg";
                    string outputPath = Path.Combine(Path.GetTempPath(), text);
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath);
                    }
                    using (FileStream fileStream = File.Open(outputPath, FileMode.OpenOrCreate))
                    {
                        builder.Save(fileStream);
                    }
                    jObj = new JObject();
                    jObj["F_GUID"] = guid;
                    jObj["F_CCLX"] = "FILE";
                    jObj["MDLID"] = "PROCESS_ALLOCATION";
                    jObj["YWLX"] = "";
                    jObj["F_NAME"] = text;
                    jObj["F_PATH"] = "";
                    jObj["EXT_STR01"] = "";
                    jObj["EXT_STR02"] = "";
                    jObj["EXT_STR03"] = "";
                    jObj["EXT_STR04"] = "";
                    var httpResponseMessage = await upload_affix.PostMultipartAsync(mp =>
                    {
                        mp.AddFile("FILE", outputPath, null, 4096, null).AddJson("AFFIXMSG", jObj);
                    });
                    File.Delete(outputPath);
                    if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        Librarys.Logger.Debug("上传文件出错！", logger);
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch (Exception message)
            {
                Librarys.Logger.Debug(message, logger);
                result = false;
            }
            return result;
        }
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
