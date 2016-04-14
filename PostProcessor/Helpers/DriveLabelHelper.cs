using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace PostProcessor.Helpers
{
	public static class DriveLabelHelper
	{
		public static string GetDrive(string label)
		{
			if (string.IsNullOrWhiteSpace(label))
			{
				return null;
			}

			foreach (DriveInfo drive in DriveInfo.GetDrives())
			{
				try
				{
					string lbl = drive.VolumeLabel;
					if (lbl == label)
					{
						return drive.Name;
					}
				}
				catch (Exception) { }
			}
			return null;
		}
	}
}
