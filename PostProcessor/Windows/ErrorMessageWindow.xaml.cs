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
	/// Interaction logic for ErrorMessageWindow.xaml
	/// </summary>
	public partial class ErrorMessageWindow : Window
	{
		public OnProgramErrorActionType Action
		{
			get;
			private set;
		}

		public bool RememberAnswer
		{
			get { return cbRememberAnswer.IsChecked == true; }
		}


		public ErrorMessageWindow(string message)
		{
			InitializeComponent();

			txtErrorMessage.Text = message;
		}


		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
			Action = OnProgramErrorActionType.Send;
			DialogResult = true;
		}

		private void btnIgnore_Click(object sender, RoutedEventArgs e)
		{
			Action = OnProgramErrorActionType.Ignore;
			DialogResult = true;
		}
	}
}
