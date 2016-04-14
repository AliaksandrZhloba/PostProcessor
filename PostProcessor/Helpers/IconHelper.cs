using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace PostProcessor.Helpers
{
	public static class IconHelper
	{
		public static Icon Convert(Bitmap bmp)
		{
			Icon ico = Icon.FromHandle((bmp).GetHicon());
			return ico;
		}
	}
}
