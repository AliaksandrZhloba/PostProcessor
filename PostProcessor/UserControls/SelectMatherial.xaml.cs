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
using System.ComponentModel;
using System.Collections;

using PostProcessor.Entity;
using PostProcessor.Helpers;


namespace PostProcessor.UserControls
{
	/// <summary>
	/// Interaction logic for SelectMatherial.xaml
	/// </summary>
	public partial class SelectMatherial : UserControl
	{
		public static readonly DependencyProperty
			MatherialsProperty = DependencyProperty.Register("Matherials", typeof(IEnumerable), typeof(SelectMatherial)),
			SourceMatherialProperty = DependencyProperty.Register("SourceMatherial", typeof(Matherial), typeof(SelectMatherial)),
			CurrentMatherialProperty = DependencyProperty.Register("CurrentMatherial", typeof(Matherial), typeof(SelectMatherial));


		public IEnumerable Matherials
		{
			get { return (IEnumerable)GetValue(MatherialsProperty); }
			set { SetValue(MatherialsProperty, value); }
		}

		public Matherial SourceMatherial
		{
			get { return (Matherial)GetValue(SourceMatherialProperty); }
			set { SetValue(SourceMatherialProperty, value); }
		}

		public Matherial CurrentMatherial
		{
			get { return (Matherial)GetValue(CurrentMatherialProperty); }
			set { SetValue(CurrentMatherialProperty, value); }
		}


		public SelectMatherial()
		{
			InitializeComponent();
		}


		public void MarkInvalidMatherial()
		{
			mvCurrentMatherial.MarkInvalidMatherial();
		}


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			cbMatherial.SelectionChanged += cbMatherial_SelectionChanged;
		}


		private void SetMatherialDefaultsButton_Click(object sender, RoutedEventArgs e)
		{
			if (SourceMatherial != null)
			{
				CurrentMatherial = SourceMatherial.Clone();
			}
		}

		private void cbMatherial_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CurrentMatherial = SourceMatherial.Clone();
		}
	}
}
