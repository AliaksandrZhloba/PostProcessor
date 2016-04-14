using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Interop;
using System.IO;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Net.Mail;

using PostProcessor.UserControls;
using PostProcessor.Helpers;
using PostProcessor.Entity;
using PostProcessor.Windows;


namespace PostProcessor
{
	public class PostProcessorApp
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

		private string _appPath;


		public bool IsBusy
		{
			get;
			private set;
		}


		public PostProcessorApp()
		{
			_appPath = System.IO.Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
			string dir = AppDomain.CurrentDomain.BaseDirectory;

			LogHelper.Initialize(dir);

			AppDomain.CurrentDomain.UnhandledException += PostProcessor_UnhandledException;


			_settings = new Settings();
			_settings.Load();
			CheckSettings();


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


		private void PostProcessor_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception exception = e.ExceptionObject as Exception;
			if (exception != null)
			{
				LogHelper.Logger.Fatal("Unhandled Exception");
				LogHelper.Logger.Fatal("Message: {0}", exception.Message);
				LogHelper.Logger.Fatal("StackTrace: {0}", exception.StackTrace);

				string message = exception.Message + Environment.NewLine + exception.StackTrace;

				switch (_settings.Program.OnProgramErrorAction)
				{
					case OnProgramErrorActionType.Ask:
						ErrorMessageWindow errorWindow = new ErrorMessageWindow(message);
						if (errorWindow.ShowDialog() == true)
						{
							if (errorWindow.RememberAnswer)
							{
								_settings.Program.OnProgramErrorAction = errorWindow.Action;
								_settings.Save();
							}

							if (errorWindow.Action == OnProgramErrorActionType.Send)
							{
								SendErrorMessage(message);
							}
						}
						break;

					case OnProgramErrorActionType.Send:
						SendErrorMessage(message);
						break;

					case OnProgramErrorActionType.Ignore:
						break;
				}
			}
			else
			{
				string message = null;
				if (e.ExceptionObject != null)
				{
					message = e.ExceptionObject.ToString();
				}
				LogHelper.Logger.Fatal(message, "Unhandled Exception, Unknown ExceptionObject");
			}

			LogHelper.Logger.Trace("Terminating...");
			LogHelper.Flush();

			App.Current.Shutdown();
		}

		private static void SendErrorMessage(string message)
		{
			SendingErrorMessageWindow sendingErrorWindow = null;

			Task.Factory.StartNew(() =>
			{
				LogHelper.Logger.Trace("Sending mail.");
				try
				{
					MailMessage mail = new MailMessage();
					SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

					// TODO Hide password

					mail.From = new MailAddress("appserrormessages@gmail.com");
					mail.To.Add("appserrormessages@gmail.com");
					mail.Subject = "PostProcessor";
					mail.Body = DateTime.Now.ToString() + Environment.NewLine + Environment.NewLine + message;
					string logfilename = LogHelper.GetLogFileName();
					if (!string.IsNullOrEmpty(logfilename))
					{
						try
						{
							mail.Attachments.Add(new Attachment(logfilename));
						}
						catch (Exception attachingException)
						{
							mail.Body += Environment.NewLine + Environment.NewLine + "Failed attaching log file.";
							LogHelper.Logger.Fatal("Error attaching file: {0}", attachingException.Message);
						}
					}

					SmtpServer.Port = 587;
					SmtpServer.Credentials = new System.Net.NetworkCredential("appserrormessages", "aempassword");
					SmtpServer.EnableSsl = true;

					SmtpServer.Send(mail);

					LogHelper.Logger.Trace("Mail was sent.");
				}
				catch (Exception ex)
				{
					LogHelper.Logger.Fatal("Error sending mail: {0}", ex.Message);
				}
			})
			.ContinueWith((Task task) =>
			{
				if (sendingErrorWindow != null)
				{
					sendingErrorWindow.Dispatcher.BeginInvoke(new Action(() =>
					{
						sendingErrorWindow.Close();
					}));
				}
			});

			sendingErrorWindow = new SendingErrorMessageWindow();
			sendingErrorWindow.ShowDialog();
		}

		public void ShowSettings()
		{
			if (_wSettings == null)
			{
				_settings.Program.AutoStart = AutorunHelper.IsRegisteredAutorun();

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
			if (_settings.Program.AutoStart)
			{
				AutorunHelper.RegisterAutorun(_appPath);
			}
			else
			{
				AutorunHelper.UnregisterAutorun();
			}

			/*if (_settings.Program.AssosiateFiles)
			{
				FileAssociateHelper.Associate("U00", "PostProcessor data file", "Post processor", _appPath, string.Empty);
			}
			else
			{
				FileAssociateHelper.RemoveAssociation("Post processor", "U00");
			}*/

			/*if (_settings.Program.AssosiateFiles)
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
			CheckWatchDir();
		}

		private void CheckWatchDir()
		{
			if (_settings.Program.WatchDir && Directory.Exists(_settings.Program.WatchDirPath))
			{
				Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

				FileWatcher.Start(_settings.Program.WatchDirPath, PostProcessor.Properties.Settings.Default.FileExtension, (string path) =>
					{
						switch (_settings.Program.OnFileDetectedAction)
						{
							case OnFileDetectedActionType.ShowMessage:
								dispatcher.Invoke(new Action(() => NotifyFileDetected(path)));
								break;

							case OnFileDetectedActionType.StartProcess:
								dispatcher.Invoke(new Action(() => ProcessFile(path)));
								break;
						}
					});
			}
			else
			{
				FileWatcher.Stop();
				_settings.Program.WatchDir = false;
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
			FileWatcher.Stop();

			_tIcon.Hide();
			_settings.Save();

			string dir = AppDomain.CurrentDomain.BaseDirectory;

			InstrumentHelper.SaveInstruments(Path.Combine(dir, INSTRUMENTS_FILE), _instruments);
			MatherialHelper.SaveMatherials(Path.Combine(dir, MATHERIALS_FILE), _matherials);

			LogHelper.Logger.Trace("Exit PostProcessor app.");

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
