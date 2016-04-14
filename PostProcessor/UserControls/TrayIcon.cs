using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;

using PostProcessor.Helpers;


namespace PostProcessor.UserControls
{
	public class TrayIcon
	{
		public Action LeftMouseButtonDoubleClicked;
		public Action LeftMouseButtonClicked;

		public Action BalloonTipClicked;
		public Action BalloonTipClosed;

		private NotifyIcon _nIcon;


		public Icon Icon
		{
			get { return _nIcon.Icon; }
			set { _nIcon.Icon = value; }
		}


		public TrayIcon(Icon icon, string title, ContextMenuStrip menu)
		{
			_nIcon = new System.Windows.Forms.NotifyIcon()
			{
				Icon = icon,
				Text = title,
				ContextMenuStrip = menu
			};

			_nIcon.BalloonTipClicked += (object nBlnClickedSender, EventArgs nBlnClickedArgs) =>
				{
					if (BalloonTipClicked != null)
					{
						BalloonTipClicked();
					}
				};
			/*_nIcon.BalloonTipClosed += (object nBlnClosedSender, EventArgs nBlnClosedArgs) =>
				{
					if (BalloonTipClosed != null)
					{
						BalloonTipClosed();
					}
				};*/

			_nIcon.MouseDoubleClick += (object nDblClckIconSender, MouseEventArgs nMDblClckArgs) =>
			{
				if (nMDblClckArgs.Button == System.Windows.Forms.MouseButtons.Left)
				{
					LeftMouseButtonDoubleClicked();
				}
			};

			_nIcon.MouseClick += (object nClckIconSender, MouseEventArgs nClckArgs) =>
			{
				if (nClckArgs.Button == MouseButtons.Left && LeftMouseButtonClicked != null)
				{
					LeftMouseButtonClicked();
				}
			};
		}


		public void ShowBallonTip(int ms, string title, string text)
		{
			_nIcon.ShowBalloonTip(3000, title, text, System.Windows.Forms.ToolTipIcon.Info);
		}


		public void Show()
		{
			_nIcon.Visible = true;
		}

		public void Hide()
		{
			_nIcon.Visible = false;
		}
	}
}
