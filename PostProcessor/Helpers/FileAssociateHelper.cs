using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.Win32;

using Associations;


namespace PostProcessor.Helpers
{
	public static class FileAssociateHelper
	{
		[DllImport("shell32.dll", SetLastError = true)]
		private static extern void SHChangeNotify(long wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);


		private const long SHCNE_ASSOCCHANGED = 0x8000000L;
		private const uint SHCNF_IDLIST = 0x0U;


		public static void Associate(string extension, string description, string productName, string path, string icon)
		{
			Registry.ClassesRoot.CreateSubKey(extension).SetValue(string.Empty, productName);

			if (!string.IsNullOrWhiteSpace(productName))
			{
				using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(productName))
				{
					if (description != null)
						key.SetValue(string.Empty, description);

					/*if (icon != null)
						key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));*/

					key.CreateSubKey(@"Shell\Open\Command").SetValue(string.Empty, path + " \"%1\"");
				}
			}

			SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
		}

		public static bool IsAssociated(string extension)
		{
			return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
		}

		public static void RemoveAssociation(string productName, string extension)
		{
			Registry.ClassesRoot.DeleteSubKeyTree(extension);
			Registry.ClassesRoot.DeleteSubKeyTree(productName);
		}
	}
}
