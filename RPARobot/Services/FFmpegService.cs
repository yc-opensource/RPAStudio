using log4net;
using RPARobot.Librarys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Services
{
    /// <summary>
    /// 使用FFmpeg录制屏幕
    /// </summary>
    public class FFmpegService
    {
        private string FFmpegPath
        {
            get
            {
                return Environment.CurrentDirectory + "\\ffmpeg.exe";
            }
        }
        private string CaptureScreenOption =>
            $" -y -rtbufsize 150M -f gdigrab -framerate {_fps}  -draw_mouse 1 -i desktop -c:v libx264 -r {_fps} -preset ultrafast -tune zerolatency -crf {_crf} -pix_fmt yuv420p -movflags +faststart ";


        public FFmpegService(string screenCaptureSavePath, int fps, int quality)
        {
            _screenCaptureSavePath = screenCaptureSavePath;
            _fps = fps;
            _crf = (100 - quality) / 2;
        }

        public void StartCaptureScreen()
        {
            if (_cliManager != null)
            {
                _cliManager.Dispose();
                _cliManager = null;
            }
            _cliManager = new CLIManager();
            string args =$"{CaptureScreenOption} \"{_screenCaptureSavePath}\"";
            Logger.Debug("ffmpeg录像开始，参数=" + args, logger);
            _cliManager.Open(FFmpegPath, args);
        }

        public bool IsRunning()
        {
            return _cliManager != null && _cliManager.IsProcessRunning();
        }

        public void StopCaptureScreen()
        {
            if (_cliManager != null)
            {
                _cliManager.WaitForClose("q");
                _cliManager.Dispose();
                _cliManager = null;
                Logger.Debug("ffmpeg录像结束", logger);
            }
        }

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string _screenCaptureSavePath;

        private CLIManager _cliManager;

        private int _fps;

        private int _crf;
    }
}
