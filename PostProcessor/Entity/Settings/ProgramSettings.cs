using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace PostProcessor.Entity
{
	public class ProgramSettings : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		public bool AutoStart
		{
			get;
			set;
		}

		public bool WatchDir
		{
			get;
			set;
		}


		private string _watchDirPath;

		public string WatchDirPath
		{
			get { return _watchDirPath; }
			set
			{
				_watchDirPath = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("WatchDirPath"));
				}
			}
		}


		private OnFileDetectedActionType _onFileDetectedAction = OnFileDetectedActionType.StartProcess;

		public OnFileDetectedActionType OnFileDetectedAction
		{
			get { return _onFileDetectedAction; }
			set
			{
				_onFileDetectedAction = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("OnFileDetectedAction"));
				}
			}
		}


		private OnProgramErrorActionType _onProgramErrorAction = OnProgramErrorActionType.Ask;

		public OnProgramErrorActionType OnProgramErrorAction
		{
			get { return _onProgramErrorAction; }
			set
			{
				_onProgramErrorAction = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("OnProgramErrorAction"));
				}
			}
		}


		public bool AssosiateFiles
		{
			get;
			set;
		}


		public bool StayInTray
		{
			get;
			set;
		}


		public static ProgramSettings Clone(ProgramSettings source)
		{
			return new ProgramSettings
			{
				AutoStart = source.AutoStart,
				WatchDir = source.WatchDir,
				WatchDirPath = source.WatchDirPath,
				AssosiateFiles = source.AssosiateFiles,
				OnFileDetectedAction = source.OnFileDetectedAction,
				StayInTray = source.StayInTray,
				OnProgramErrorAction = source.OnProgramErrorAction
			};
		}
	}
}
