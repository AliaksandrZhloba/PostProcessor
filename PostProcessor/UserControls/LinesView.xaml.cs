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

namespace PostProcessor.UserControls
{
	/// <summary>
	/// Interaction logic for LinesView.xaml
	/// </summary>
	public partial class LinesView : UserControl
	{
		public LinesView()
		{
			InitializeComponent();
		}


		public void SetLines(string[] lines, int lnum)
		{
			FlowDocument document = new FlowDocument();
			Paragraph paragraph = new Paragraph();

			for (int i = Math.Max(lnum - 3, 0); i < Math.Min(lnum + 3, lines.Length - 1); i++)
			{
				Run txtLine = new Run(lines[i] + Environment.NewLine);
				if (i == lnum)
				{
					txtLine.Foreground = Brushes.Red;
				}
				paragraph.Inlines.Add(txtLine);
			}

			document.Blocks.Add(paragraph);
			txtLines.Document = document;
		}
	}
}
