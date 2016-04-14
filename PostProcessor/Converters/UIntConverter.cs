using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;


namespace PostProcessor.Converters
{
	public class UIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			uint val = (uint)value;
			return val.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string txt = (string)value;
			uint rez = 0;
			if (uint.TryParse(txt, out rez))
			{
				return rez;
			}
			return null;
		}
	}
}
