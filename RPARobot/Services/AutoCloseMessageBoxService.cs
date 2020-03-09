using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RPARobot.Services
{
	public class AutoCloseMessageBoxService
	{
		public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult = MessageBoxResult.Yes)
		{
			return (MessageBoxResult)AutoClosingMessageBox.Factory((string _caption, MessageBoxButtons _buttons) => System.Windows.Forms.MessageBox.Show(new WindowWrapper(owner), messageBoxText, _caption, _buttons, (MessageBoxIcon)icon), caption).Show(AutoCloseMessageBoxService.Timeout, (MessageBoxButtons)button, (DialogResult)defaultResult);
		}

		public static MessageBoxResult Show(string messageBoxText)
		{
			return (MessageBoxResult)AutoClosingMessageBox.Factory((string _caption, MessageBoxButtons _buttons) => System.Windows.Forms.MessageBox.Show(messageBoxText), null).Show(AutoCloseMessageBoxService.Timeout, MessageBoxButtons.OK, DialogResult.None);
		}

		private static int Timeout = 10000;
	}
}
