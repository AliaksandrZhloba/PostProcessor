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

using PostProcessor.Helpers;


namespace PostProcessor.UserControls
{
	/// <summary>
	/// Interaction logic for FilePathView.xaml
	/// </summary>
	public partial class FilePathView : UserControl
	{
		public static readonly DependencyProperty
			FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(FilePathView));

		public string FilePath
		{
			get { return (string)GetValue(FilePathProperty); }
			set { SetValue(FilePathProperty, value); }
		}


		private System.Windows.Forms.OpenFileDialog _openFile;


		public string FileFilter
		{
			get { return _openFile.Filter; }
			set { _openFile.Filter = value; }
		}


		public FilePathView()
		{
			InitializeComponent();

			_openFile = new System.Windows.Forms.OpenFileDialog();
		}


		public void MarkInvalidPath()
		{
			AnimationHelper.AnimateTextBox(txtPath);
		}


		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			_openFile.FileName = FilePath;
			try
			{
				_openFile.InitialDirectory = System.IO.Path.GetDirectoryName(FilePath);
			}
			catch (ArgumentException)
			{ }
			catch (PathTooLongException)
			{ }

			if (_openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				FilePath = _openFile.FileName;
			}
		}
	}
}
