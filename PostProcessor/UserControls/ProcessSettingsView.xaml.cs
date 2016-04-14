using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using PostProcessor.Helpers;
using PostProcessor.Entity;


namespace PostProcessor.UserControls
{
	/// <summary>
	/// Interaction logic for ProcessSettingsView.xaml
	/// </summary>
	public partial class ProcessSettingsView : UserControl
	{
		public static readonly DependencyProperty
			SettingsProperty = DependencyProperty.Register("Settings", typeof(ProcessSettings), typeof(ProcessSettingsView));

		public ProcessSettings Settings
		{
			get { return (ProcessSettings)GetValue(SettingsProperty); }
			set { SetValue(SettingsProperty, value); }
		}



		private ContextMenu _cmFlashDrives;
		private CancellationTokenSource _cts;
		private Brush _normalTextBrush;


		public ProcessSettingsView()
		{
			InitializeComponent();

			_normalTextBrush = txtUSBLabel.Foreground;

			_cmFlashDrives = new ContextMenu();
			_cmFlashDrives.Style = (Style)FindResource("FlashDrivesContextMenuStyle");

			btnBrowse.ContextMenu = _cmFlashDrives;
		}


		public void MarkInvalidUSBLabel()
		{
			AnimationHelper.AnimateTextBox(txtUSBLabel);
		}

		public void MarkInvalidXEnd()
		{
			AnimationHelper.AnimateTextBox(txtXEnd);
		}

		public void MarkInvalidYEnd()
		{
			AnimationHelper.AnimateTextBox(txtYEnd);
		}


		private void btnBrowse_Click(object sender, RoutedEventArgs e)
		{
			_cmFlashDrives.Items.Clear();

			foreach (DriveInfo drive in DriveInfo.GetDrives())
			{
				if (drive.DriveType == DriveType.Removable)
				{
					try
					{
						string label = drive.VolumeLabel;

						MenuItem item = new MenuItem() { DataContext = drive };
						item.Click += (i_sender, i_e) =>
						{
							Settings.USBLabel = drive.VolumeLabel;
						};

						_cmFlashDrives.Items.Add(item);
					}
					catch (IOException) { }
				}
			}

			if (_cmFlashDrives.Items.Count != 0)
			{
				_cmFlashDrives.IsOpen = true;
			}
		}


		public void StartSearchUSB()
		{
			LogHelper.Logger.Trace("Start searching usb.");

			_cts = new CancellationTokenSource();
			Task.Factory.StartNew((Action)(() =>
			{
				string tmp = null;
				bool found = false;
				string label = null;
				while (true)
				{
					found = false;
					Dispatcher.Invoke((Action)(() =>
					{
						label = txtUSBLabel.Text;
					}));

					foreach (DriveInfo drive in DriveInfo.GetDrives())
					{
						if (drive.DriveType == DriveType.Removable)
						{
							try
							{
								tmp = drive.VolumeLabel;
							}
							catch (IOException) { }
							if (tmp == label)
							{
								found = true;
								break;
							}
						}
					}
					Dispatcher.Invoke((Action)(() =>
					{
						txtUSBLabel.Foreground = found ? Brushes.Green : Brushes.Red; ;
					}));

					if (_cts.IsCancellationRequested)
					{
						break;
					}
					Thread.Sleep(250);
				}
			}));
		}

		public void StopSearchUSB()
		{
			LogHelper.Logger.Trace("Stop searching usb.");
			_cts.Cancel();
		}


		private void txtUSBLabel_TextChanged(object sender, TextChangedEventArgs e)
		{
			txtUSBLabel.Foreground = _normalTextBrush;
		}
	}
}
