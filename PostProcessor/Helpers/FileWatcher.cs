using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace PostProcessor.Helpers
{
	public static class FileWatcher
	{
		private static FileSystemWatcher _fWatcher;

		public static void Start(string path, string extension, Action<string> fileDetected)
		{
			if (_fWatcher != null)
			{
				_fWatcher.EnableRaisingEvents = false;
				_fWatcher.Dispose();
			}

			string filter = string.Format("*.{0}", extension);
			_fWatcher = new FileSystemWatcher() { Filter = filter, Path = path };
			_fWatcher.Changed += (object sender, FileSystemEventArgs e) =>
			{
				fileDetected(e.FullPath);
			};

			_fWatcher.EnableRaisingEvents = true;
		}

		public static void Stop()
		{
			if (_fWatcher != null)
			{
				_fWatcher.EnableRaisingEvents = false;
				_fWatcher.Dispose();
			}
		}

		public static void Pause()
		{
			if (_fWatcher != null)
			{
				_fWatcher.EnableRaisingEvents = false;
			}
		}

		public static void Continue()
		{
			if (_fWatcher != null)
			{
				_fWatcher.EnableRaisingEvents = true;
			}
		}
	}
}
