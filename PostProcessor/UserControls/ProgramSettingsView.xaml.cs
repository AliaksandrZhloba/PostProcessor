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
using f = System.Windows.Forms;

using PostProcessor.Entity;


namespace PostProcessor.UserControls
{
	/// <summary>
	/// Interaction logic for ProgramSettingsView.xaml
	/// </summary>
	public partial class ProgramSettingsView : UserControl
	{
		public static readonly DependencyProperty
			SettingsProperty = DependencyProperty.Register("Settings", typeof(ProgramSettings), typeof(ProgramSettingsView));

		public ProgramSettings Settings
		{
			get { return (ProgramSettings)GetValue(SettingsProperty); }
			set { SetValue(SettingsProperty, value); }
		}


		public ProgramSettingsView()
		{
			InitializeComponent();

			txtExtension.Text = PostProcessor.Properties.Settings.Default.FileExtension;
		}


		private f.FolderBrowserDialog _browseDialog;

		private void BrowseDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			if (_browseDialog == null)
			{
				_browseDialog = new f.FolderBrowserDialog();
				_browseDialog.SelectedPath = Settings.WatchDirPath;
			}

			if (_browseDialog.ShowDialog() == f.DialogResult.OK)
			{
				Settings.WatchDirPath = _browseDialog.SelectedPath;
			}
		}
	}
}
