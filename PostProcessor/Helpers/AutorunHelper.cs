using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;


namespace PostProcessor.Helpers
{
	public static class AutorunHelper
	{
		private static string APP_NAME = "PostProcessor";

		// The path to the key where Windows looks for startup applications
		private static RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);



		public static bool IsRegisteredAutorun()
		{
			return (rkApp.GetValue(APP_NAME) != null);
		}


		public static void RegisterAutorun(string path)
		{
			rkApp.SetValue(APP_NAME, path);
		}

		public static void UnregisterAutorun()
		{
			rkApp.DeleteValue(APP_NAME, false);
		}
	}
}
