using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PostProcessor.Entity;
using PostProcessor.Helpers;


namespace PostProcessor.UserControls
{
	/// <summary>
	/// Interaction logic for MatherialView.xaml
	/// </summary>
	public partial class MatherialView : UserControl
	{
		public MatherialView()
		{
			InitializeComponent();
		}


		public void MarkInvalidMatherial()
		{
			AnimationHelper.AnimateTextBox(txtSpeed);
			AnimationHelper.AnimateTextBox(txtFxy);
			AnimationHelper.AnimateTextBox(txtFz);
		}

		private void txtSpeed_GotFocus(object sender, RoutedEventArgs e)
		{
			txtSpeed.SelectAll();
		}

		private void txtFxy_GotFocus(object sender, RoutedEventArgs e)
		{
			txtFxy.SelectAll();
		}

		private void txtFz_GotFocus(object sender, RoutedEventArgs e)
		{
			txtFz.SelectAll();
		}

		/*private void MatherialView_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				if (txtSpeed.IsFocused)
				{
					txtFxy.Focus();
					txtFxy.SelectAll();
					e.Handled = true;
				}
				else if (txtFxy.IsFocused)
				{
					txtFz.Focus();
					txtFz.SelectAll();
					e.Handled = true;
				}
				else if (txtFz.IsFocused)
				{
					txtSpeed.Focus();
					txtSpeed.SelectAll();
					e.Handled = true;
				}
			}
		}*/
	}
}
