using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for MatherialsWindow.xaml
	/// </summary>
	public partial class MatherialsWindow : Window
	{
		public MatherialsWindow(BindingList<Matherial> copy)
		{
			InitializeComponent();
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
