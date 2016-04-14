using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;


namespace PostProcessor.Helpers
{
	public static class LogHelper
	{
		public static Logger Logger
		{
			get;
			private set;
		}


		public static void Initialize(string dir)
		{
			FileTarget target = LogManager.Configuration.FindTargetByName("FileLogger") as FileTarget;
			string filename = target.FileName.ToString().Trim('\'');
			target.FileName = Path.Combine(dir, filename);


			Logger = LogManager.GetCurrentClassLogger();
			LogHelper.Logger.Trace("==================== Run PostProcessor app. ====================");
			Logger.Trace("Version: {0}", Environment.Version.ToString());
			Logger.Trace("OS: {0}", Environment.OSVersion.ToString());
			Logger.Trace("Command: {0}", Environment.CommandLine.ToString());
		}


		public static string GetLogFileName()
		{
			string targetName = "FileLogger";
			string fileName = null;

			if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
			{
				Target target = LogManager.Configuration.FindTargetByName(targetName);
				if (target == null)
				{
					return null;
				}

				FileTarget fileTarget = null;
				WrapperTargetBase wrapperTarget = target as WrapperTargetBase;

				// Unwrap the target if necessary.
				if (wrapperTarget == null)
				{
					fileTarget = target as FileTarget;
				}
				else
				{
					fileTarget = wrapperTarget.WrappedTarget as FileTarget;
				}

				if (fileTarget == null)
				{
					return null;
				}

				var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
				fileName = fileTarget.FileName.Render(logEventInfo);
			}
			else
			{
				throw new Exception("LogManager contains no Configuration or there are no named targets");
			}

			if (!File.Exists(fileName))
			{
				return null;
			}

			return fileName;
		}


		public static void Flush()
		{
			LogManager.Flush();
		}
	}
}
