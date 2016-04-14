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
using System.Windows.Shapes;

using PostProcessor.Entity;


namespace PostProcessor.Windows
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public Action Applyed, Canceled;


		public SettingsWindow(ProgramSettings settings)
		{
			InitializeComponent();

			programSettingsView.Settings = settings;
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Canceled();
			Close();
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			Applyed();
			Close();
		}
	}
}
