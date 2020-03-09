using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace RPARobot.Services
{
	public class WindowWrapper : System.Windows.Forms.IWin32Window
	{
		public WindowWrapper(IntPtr handle)
		{
			this._hwnd = handle;
		}

		public WindowWrapper(Window window)
		{
			this._hwnd = new WindowInteropHelper(window).Handle;
		}

		public IntPtr Handle
		{
			get
			{
				return this._hwnd;
			}
		}

		private IntPtr _hwnd;
	}
}
