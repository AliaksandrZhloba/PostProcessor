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

using PostProcessor.Helpers;


namespace PostProcessor.Windows
{
	/// <summary>
	/// Interaction logic for InputFValueWindow.xaml
	/// </summary>
	public partial class InputFValueWindow : Window
	{
		public enum InputFValueResults
		{
			Undefined,
			Apply,
			ApplyForAll,
			Ignore,
			IgnoreAll,
			Abort
		}


		public InputFValueResults InputFValueResult
		{
			get;
			private set;
		}


		private int _fValue = 0;


		public bool Remember
		{
			get { return cbRemember.IsChecked == true; }
		}

		public int FValue
		{
			get { return _fValue; }
		}


		public InputFValueWindow(int f, int lnum, string[] lines)
		{
			InitializeComponent();

			InputFValueResult = InputFValueResults.Undefined;

			txtLineNum.Text = (lnum + 1).ToString();
			txtFValue.Text = f.ToString();
			linesView.SetLines(lines, lnum);
		}


		private void InputFValueWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (InputFValueResult != InputFValueResults.Undefined)
			{
				return;
			}

			if (MessageBox.Show("Прервать обработку?", "Внимание", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
			{
				InputFValueResult = InputFValueResults.Abort;
			}
			else
			{
				e.Cancel = true;
			}
		}


		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			if (!int.TryParse(txtFValue.Text, out _fValue))
			{
				AnimationHelper.AnimateTextBox(txtFValue);
			}
			else
			{
				InputFValueResult = InputFValueResults.Apply;
				Close();
			}
		}


		private void IgnoreButton_Click(object sender, RoutedEventArgs e)
		{
			InputFValueResult = InputFValueResults.Ignore;
			Close();
		}

		private void IgnoreAllButton_Click(object sender, RoutedEventArgs e)
		{
			InputFValueResult = InputFValueResults.IgnoreAll;
			Close();
		}
	}
}
