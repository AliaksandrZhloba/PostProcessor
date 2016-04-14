using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace PostProcessor.Helpers
{
	public static class AnimationHelper
	{
		public static void AnimateTextBox(TextBox textbox)
		{
			textbox.Background = new SolidColorBrush();

			ColorAnimation ca = new ColorAnimation(Colors.Red, new Duration(TimeSpan.FromMilliseconds(50)));
			ca.AutoReverse = true;
			ca.RepeatBehavior = new RepeatBehavior(4);

			textbox.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
		}
	}
}
