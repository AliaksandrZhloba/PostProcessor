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

namespace PostProcessor.Windows
{
	/// <summary>
	/// Interaction logic for InstrumentsWindow.xaml
	/// </summary>
	public partial class InstrumentsWindow : Window
	{
		public InstrumentsWindow(BindingList<Entity.Instrument> instruments)
		{
			InitializeComponent();
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
