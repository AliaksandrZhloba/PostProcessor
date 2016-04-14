using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PostProcessor.Entity
{
	public class Settings
	{
		public ProgramSettings Program
		{
			get;
			set;
		}

		public ProcessSettings Process
		{
			get;
			set;
		}


		public Settings()
		{
			Program = new ProgramSettings();
			Process = new ProcessSettings();
		}


		public void Load()
		{
			Program.WatchDir = PostProcessor.Properties.Settings.Default.WathDir;
			Program.WatchDirPath = PostProcessor.Properties.Settings.Default.WatchDirPath;
			Program.AssosiateFiles = PostProcessor.Properties.Settings.Default.AssosiateFiles;
			Program.StayInTray = PostProcessor.Properties.Settings.Default.StayInTray;

			OnFileDetectedActionType onFileDetectedAction = OnFileDetectedActionType.StartProcess;
			Enum.TryParse<OnFileDetectedActionType>(PostProcessor.Properties.Settings.Default.OnFileDetectedAction, out onFileDetectedAction);
			Program.OnFileDetectedAction = onFileDetectedAction;

			OnProgramErrorActionType onProgramErrorAction = OnProgramErrorActionType.Ask;
			Enum.TryParse<OnProgramErrorActionType>(PostProcessor.Properties.Settings.Default.OnProgramErrorAction, out onProgramErrorAction);
			Program.OnProgramErrorAction = onProgramErrorAction;

			Program.AutoStart = Helpers.AutorunHelper.IsRegisteredAutorun();


			Process.FilePath = PostProcessor.Properties.Settings.Default.FilePath;
			Process.ControlVC = PostProcessor.Properties.Settings.Default.ControlVC;
			Process.SendToUSB = PostProcessor.Properties.Settings.Default.SendToUSB;
			Process.USBLabel = PostProcessor.Properties.Settings.Default.USBLabel;
			Process.ClearUSB = PostProcessor.Properties.Settings.Default.ClearUSB;
			Process.GotoXYZ = PostProcessor.Properties.Settings.Default.GotoXYZ;
			Process.XEnd = PostProcessor.Properties.Settings.Default.XEnd;
			Process.YEnd = PostProcessor.Properties.Settings.Default.YEnd;
			Process.ZEnd = PostProcessor.Properties.Settings.Default.ZEnd;
			Process.ShowInstrumentsSequence = PostProcessor.Properties.Settings.Default.ShowInstrumentsSequence;
			Process.SourceMatherial = Matherial.Deserialize(PostProcessor.Properties.Settings.Default.SourceMatherial);
			Process.CurrentMatherial = Matherial.Deserialize(PostProcessor.Properties.Settings.Default.CurrentMatherial);
		}


		public void Save()
		{
			PostProcessor.Properties.Settings.Default.WathDir = Program.WatchDir;
			PostProcessor.Properties.Settings.Default.WatchDirPath = Program.WatchDirPath;
			PostProcessor.Properties.Settings.Default.AssosiateFiles = Program.AssosiateFiles;
			PostProcessor.Properties.Settings.Default.StayInTray = Program.StayInTray;
			PostProcessor.Properties.Settings.Default.OnFileDetectedAction = Program.OnFileDetectedAction.ToString();
			PostProcessor.Properties.Settings.Default.OnProgramErrorAction = Program.OnProgramErrorAction.ToString();

			PostProcessor.Properties.Settings.Default.FilePath = Process.FilePath;
			PostProcessor.Properties.Settings.Default.ControlVC = Process.ControlVC;
			PostProcessor.Properties.Settings.Default.SendToUSB = Process.SendToUSB;
			PostProcessor.Properties.Settings.Default.USBLabel = Process.USBLabel;
			PostProcessor.Properties.Settings.Default.ClearUSB = Process.ClearUSB;
			PostProcessor.Properties.Settings.Default.GotoXYZ = Process.GotoXYZ;
			PostProcessor.Properties.Settings.Default.XEnd = Process.XEnd;
			PostProcessor.Properties.Settings.Default.YEnd = Process.YEnd;
			PostProcessor.Properties.Settings.Default.ZEnd = Process.ZEnd;
			PostProcessor.Properties.Settings.Default.ShowInstrumentsSequence = Process.ShowInstrumentsSequence;
			PostProcessor.Properties.Settings.Default.CurrentMatherial = Matherial.Serialize(Process.CurrentMatherial);
			PostProcessor.Properties.Settings.Default.SourceMatherial = Matherial.Serialize(Process.SourceMatherial);

			PostProcessor.Properties.Settings.Default.Save();
		}
	}
}
