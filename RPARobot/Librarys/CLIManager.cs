using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPARobot.Librarys
{
	public class CLIManager : IDisposable
	{
		public event DataReceivedEventHandler OutputDataReceived;

		public event DataReceivedEventHandler ErrorDataReceived;

		public virtual int Open(string path, string args = null)
		{
			if (File.Exists(path))
			{
				using (process = new Process())
				{
					ProcessStartInfo processStartInfo = new ProcessStartInfo
					{
						FileName = path,
						WorkingDirectory = Path.GetDirectoryName(path),
						Arguments = args,
						UseShellExecute = false,
						CreateNoWindow = true,
						RedirectStandardInput = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						StandardOutputEncoding = Encoding.UTF8,
						StandardErrorEncoding = Encoding.UTF8
					};
					process.EnableRaisingEvents = true;
					if (processStartInfo.RedirectStandardOutput)
					{
						process.OutputDataReceived += cli_OutputDataReceived;
					}
					if (processStartInfo.RedirectStandardError)
					{
						process.ErrorDataReceived += cli_ErrorDataReceived;
					}
					process.StartInfo = processStartInfo;
					process.Start();
					if (processStartInfo.RedirectStandardOutput)
					{
						process.BeginOutputReadLine();
					}
					if (processStartInfo.RedirectStandardError)
					{
						process.BeginErrorReadLine();
					}
					try
					{
						processRunning = true;
						process.WaitForExit();
					}
					finally
					{
						processRunning = false;
					}
					return process.ExitCode;
				}
			}
			return -1;
		}

		public bool IsProcessRunning()
		{
			return processRunning;
		}

		public void WaitForClose(string quitCmd)
		{
			try
			{
				while (processRunning)
				{
					if (closeTryCount >= 3)
					{
						process.Kill();
						break;
					}
					WriteInput(quitCmd);
					Thread.Sleep(500);
					closeTryCount++;
				}
			}
			catch (Exception)
			{
			}
		}

		private void cli_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				DataReceivedEventHandler outputDataReceived = OutputDataReceived;
				if (outputDataReceived == null)
				{
					return;
				}
				outputDataReceived(sender, e);
			}
		}

		private void cli_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				DataReceivedEventHandler errorDataReceived = ErrorDataReceived;
				if (errorDataReceived == null)
				{
					return;
				}
				errorDataReceived(sender, e);
			}
		}

		public void WriteInput(string input)
		{
			if (processRunning && process != null && process.StartInfo != null && process.StartInfo.RedirectStandardInput)
			{
				process.StandardInput.WriteLine(input);
			}
		}

		public virtual void Close()
		{
			if (processRunning && this.process != null)
			{
				process.CloseMainWindow();
			}
		}

		public void Dispose()
		{
			if (process != null)
			{
				process.Dispose();
			}
		}

		protected Process process;

		protected bool processRunning;

		private int closeTryCount;
	}
}
