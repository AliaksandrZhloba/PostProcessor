using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Interop;
using System.IO;
using System.Windows.Threading;

using PostProcessor.UserControls;
using PostProcessor.Helpers;
using PostProcessor.Entity;
using PostProcessor.Windows;


namespace PostProcessor
{
	public class ProcessFileApp
	{
		private const string MATHERIALS_FILE = "matherials.xml";
		private const string INSTRUMENTS_FILE = "instruments.xml";


		private TrayIcon _tIcon;
		private TrayMenu _tMenu;
		private Settings _settings;

		private SettingsWindow _wSettings;
		private ProcessWindow _wProcess;

		private BindingList<Matherial> _matherials;
		private BindingList<Instrument> _instruments;

		private FileSystemWatcher _fWatcher;


		public bool IsBusy
		{
			get;
			private set;
		}


		public ProcessFileApp()
		{
			_settings = new Settings();
			_settings.Load();
			CheckSettings();

			string dir = AppDomain.CurrentDomain.BaseDirectory;

			_matherials = MatherialHelper.LoadMatherials(Path.Combine(dir, MATHERIALS_FILE));
			_instruments = InstrumentHelper.LoadInstruments(Path.Combine(dir, INSTRUMENTS_FILE));

			if (_settings.Process.SourceMatherial != null)
			{
				foreach (Matherial matherial in _matherials)
				{
					if (Matherial.Equals(_settings.Process.SourceMatherial, matherial))
					{
						_settings.Process.SourceMatherial = matherial;
						break;
					}
				}
			}


			_tMenu = new TrayMenu(_settings)
			{
				SettingsClicked = ShowSettings,
				ProcessClicked = Process,
				CloseClicked = CloseApp,
				MatherialsClicked = ShowMatherials,
				InstrumentsClicked = ShowInstruments,
			};

			_tIcon = new TrayIcon(IconHelper.Convert(PostProcessor.Properties.Resources.GreenCircle), "Post processor", _tMenu)
			{
				LeftMouseButtonDoubleClicked = Process,
			};

			IsBusy = false;
		}


		public void ShowSettings()
		{
			if (_wSettings == null)
			{
				ProgramSettings copy = ProgramSettings.Clone(_settings.Program);
				_wSettings = new SettingsWindow(copy);
				_wSettings.Closed += (object sender, EventArgs e) => _wSettings = null;
				_wSettings.Canceled = () => _wSettings = null;
				_wSettings.Applyed = () => {_settings.Program = copy; ApplySettings(); };
				_wSettings.Show();
			}
			else
			{
				_wSettings.Activate();
			}
		}

		private void ApplySettings()
		{
			/*if (_settings.Program.AutoStart)
			{
				if (!AutostartHelper.IsRegisteredAutostart(_appName))
				{
					AutostartHelper.RegisterAutostart(_appName, _appPath);
				}
			}
			else
			{
				if (AutostartHelper.IsRegisteredAutostart(_appName))
				{
					AutostartHelper.UnegisterAutostart(_appName, _appPath);
				}
			}

			if (_settings.Program.AssosiateFiles)
			{
				if (!FileAssociateHelper.IsAssociated(_fileExtension))
				{
					FileAssociateHelper.AssociateFile(_fileExtension, string.Empty, string.Empty, _appName, _appPath);
				}
			}
			else
			{
				if (FileAssociateHelper.IsAssociated(_fileExtension))
				{
					FileAssociateHelper.RemoveAssociation(_fileExtension);
				}
			}*/

			CheckWatchDir();
		}

		private void CheckSettings()
		{
			//_settings.Program.AutoStart = AutostartHelper.IsRegisteredAutostart(_appName);
			//_settings.Program.AssosiateFiles = FileAssociateHelper.IsAssociated(_fileExtension);

			CheckWatchDir();
		}

		private void CheckWatchDir()
		{
			if (_settings.Program.WatchDir && Directory.Exists(_settings.Program.WatchDirPath))
			{
				if (_fWatcher == null)
				{
					string filter = string.Format("*.{0}", PostProcessor.Properties.Settings.Default.FileExtension);
					_fWatcher = new FileSystemWatcher() { Filter = filter};
					Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
					_fWatcher.Changed += (object sender, FileSystemEventArgs e) =>
					{
						switch (_settings.Program.OnFileDetectedAction)
						{
							case OnFileDetectedActionType.ShowMessage:
								dispatcher.Invoke(new Action(() => NotifyFileDetected(e.FullPath)));
								break;

							case OnFileDetectedActionType.StartProcess:
								dispatcher.Invoke(new Action(() => ProcessFile(e.FullPath)));
								break;
						}
					};
				}

				_fWatcher.Path = _settings.Program.WatchDirPath;
				_fWatcher.EnableRaisingEvents = true;
			}
			else
			{
				_settings.Program.WatchDir = false;
				if (_fWatcher != null)
				{
					_fWatcher.EnableRaisingEvents = false;
				}
			}
		}


		public void Run()
		{
			_tIcon.Show();
		}


		public void ProcessFile(string file)
		{
			_settings.Process.FilePath = file;
			Process();
		}


		private void CloseApp()
		{
			_tIcon.Hide();
			_settings.Save();

			string dir = AppDomain.CurrentDomain.BaseDirectory;

			InstrumentHelper.SaveInstruments(Path.Combine(dir, INSTRUMENTS_FILE), _instruments);
			MatherialHelper.SaveMatherials(Path.Combine(dir, MATHERIALS_FILE), _matherials);

			Application.Current.Shutdown();
		}


		private void Process()
		{
			if (_wProcess == null)
			{
				_wProcess = new ProcessWindow(_matherials, _instruments, _settings);
				_wProcess.Closed += (object sender, EventArgs e) => _wProcess = null;
				_wProcess.Show();
			}
			else
			{
				_wProcess.Activate();
			}
		}


		private void ShowMatherials()
		{
			BindingList<Matherial> copy = MatherialHelper.Clone(_matherials);

			MatherialsWindow wMatherials = new MatherialsWindow(copy);
			if (wMatherials.ShowDialog() == true)
			{
				_matherials = copy;
			}
		}


		private void ShowInstruments()
		{
			BindingList<Instrument> copy = InstrumentHelper.Clone(_instruments);

			InstrumentsWindow wInstruments = new InstrumentsWindow(copy);
			if (wInstruments.ShowDialog() == true)
			{
				_instruments = copy;
			}
		}


		private void NotifyFileDetected(string path)
		{
			_tIcon.ShowBallonTip(5000, "Внимание", "Обнаружен новый файл");
			_tIcon.Icon = IconHelper.Convert(PostProcessor.Properties.Resources.ОrangeСircle);
			_tIcon.BalloonTipClicked = _tIcon.LeftMouseButtonClicked = () =>
				{
					_tIcon.Icon = IconHelper.Convert(PostProcessor.Properties.Resources.GreenCircle);
					_tIcon.BalloonTipClicked = _tIcon.LeftMouseButtonClicked = null;
					ProcessFile(path);
				};
			_tIcon.BalloonTipClosed = () =>
				{
					_tIcon.Icon = IconHelper.Convert(PostProcessor.Properties.Resources.GreenCircle);
					_tIcon.BalloonTipClicked = _tIcon.LeftMouseButtonClicked = null;
				};
		}
	}
}
