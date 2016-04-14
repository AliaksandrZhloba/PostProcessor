using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Threading;

using PostProcessor.Entity;
using PostProcessor.Helpers;
using PostProcessor.UserControls;


namespace PostProcessor.Windows
{
	/// <summary>
	/// Interaction logic for ProcessWindow.xaml
	/// </summary>
	public partial class ProcessWindow : Window
	{
		private ProcessSettings _processSettings;
		private Processor _processor;

		private BindingList<Instrument> _instruments;
		private List<Instrument> _instrumentSequence = new List<Instrument>();


		public ProcessWindow(BindingList<Matherial> matherials, BindingList<Instrument> instruments, Settings settings)
		{
			InitializeComponent();

			_instruments = instruments;
			processSettings.Settings = _processSettings = settings.Process;

			if (_processSettings.CurrentMatherial == null)
			{
				_processSettings.CurrentMatherial = new Matherial(string.Empty, 0, 0, 0);
			}

			selectMatherial.Matherials = matherials;
			selectMatherial.SetBinding(SelectMatherial.SourceMatherialProperty, new Binding("SourceMatherial") { Source = _processSettings, Mode = BindingMode.TwoWay });
			selectMatherial.SetBinding(SelectMatherial.CurrentMatherialProperty, new Binding("CurrentMatherial") { Source = _processSettings, Mode = BindingMode.TwoWay });

			filePathView.SetBinding(FilePathView.FilePathProperty, new Binding("FilePath") { Source = settings.Process, Mode = BindingMode.TwoWay });
			try
			{
				filePathView.FileFilter = string.Format("*.{0}|*.{0} *.*|*.*", PostProcessor.Properties.Settings.Default.FileExtension);
			}
			catch (ArgumentException) { }

			processSettings.StartSearchUSB();
		}


		private void ProcessWindow_Closing(object sender, CancelEventArgs e)
		{
			if (_processor != null && _processor.IsRunning)
			{
				MessageBox.Show("Дождитесь окончания процесса.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				e.Cancel = true;
			}
			else
			{
				processSettings.StopSearchUSB();
			}
		}


		private void ProcessButton_Click(object sender, RoutedEventArgs e)
		{
			bool valid = true;

			string path = filePathView.FilePath;
			if (!File.Exists(path))
			{
				filePathView.MarkInvalidPath();
				valid = false;
			}

			Matherial matherial = selectMatherial.CurrentMatherial;
			if (matherial == null)
			{
				selectMatherial.MarkInvalidMatherial();
				valid = false;
			}

			if (_processSettings.GotoXYZ)
			{
				if (_processSettings.XEnd > PostProcessor.Properties.Settings.Default.XEndMax)
				{
					processSettings.MarkInvalidXEnd();
					valid = false;
				}

				if (_processSettings.YEnd > PostProcessor.Properties.Settings.Default.YEndMax)
				{
					processSettings.MarkInvalidYEnd();
					valid = false;
				}
			}

			if (_processSettings.SendToUSB)
			{
				_processSettings.Drive = DriveLabelHelper.GetDrive(_processSettings.USBLabel);
				if (_processSettings.Drive == null)
				{
					processSettings.MarkInvalidUSBLabel();
					valid = false;
				}
			}

			if (valid)
			{
				ProcessFile();
			}
		}

		private void ProcessWindow_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("FileDrop"))
			{
				string[] files = (string[])e.Data.GetData("FileDrop");
				filePathView.FilePath = files[0];
			}
		}


		private void ProcessFile()
		{
			IsEnabled = false;

			Thread thread = new Thread(ProcessFileBackground);
			thread.Start();
		}

		private void ProcessFileBackground()
		{
			if (_processor == null)
			{
				Dispatcher.Invoke(new Action( () => txtStatus.Text = "Создание процессора..."));
				_processor = new Processor();
				_processor.GetFValue = Processor_GetFValue;
				_processor.ChangeInstrumentDetected = Processor_InputInstrument;
				_processor.Finished = ProcessFinished;
			}

			_instrumentSequence.Clear();
			_ignoreAllUnknownFValues = false;
			_ignoreAllUnknownInstruments = false;

			Dispatcher.Invoke(new Action(() => txtStatus.Text = "Обработка файла..."));
			_processor.Run(_processSettings);
		}


		private Dictionary<int, int> _fvals = new Dictionary<int,int>();
		private bool _ignoreAllUnknownFValues;

		private InputValueResults Processor_GetFValue(int f, int lnum, string[] lines, out int newFValue)
		{
			if (_fvals.ContainsKey(f))
			{
				newFValue = _fvals[f];
				return InputValueResults.Apply;
			}

			if (_ignoreAllUnknownFValues)
			{
				newFValue = f;
				return InputValueResults.Apply;
			}

			int fValue = newFValue = f;
			InputValueResults result = InputValueResults.Unset;
			Dispatcher.Invoke(new Action(() =>
			{
				InputFValueWindow input = new InputFValueWindow(f, lnum, lines) { Owner = this };
				input.ShowDialog();
				switch (input.InputFValueResult)
				{
					case InputFValueWindow.InputFValueResults.Apply:
						fValue = input.FValue;
						_fvals.Add(f, input.FValue);
						result = InputValueResults.Apply;
						break;

					case InputFValueWindow.InputFValueResults.Ignore:
						result = InputValueResults.Apply;
						break;

					case InputFValueWindow.InputFValueResults.IgnoreAll:
						_ignoreAllUnknownFValues = true;
						result = InputValueResults.Apply;
						break;

					case InputFValueWindow.InputFValueResults.Abort:
						result = InputValueResults.Abort;
						break;
				}
			}));

			newFValue = fValue;
			return result;
		}



		private bool _ignoreAllUnknownInstruments;

		private InputValueResults Processor_InputInstrument(int t, int lnum, string[] lines)
		{
			foreach (Instrument instrument in _instruments)
			{
				if (instrument.Num == t)
				{
					_instrumentSequence.Add(instrument);
					return InputValueResults.Apply;
				}
			}

			if (_ignoreAllUnknownInstruments)
			{
				_instrumentSequence.Add(new Instrument(t, string.Empty));
				return InputValueResults.Apply;
			}

			InputValueResults result = InputValueResults.Unset;
			Dispatcher.Invoke(new Action(() =>
			{
				InputInstrumentWindow inputInstrument = new InputInstrumentWindow(t, lnum, lines) { Owner = this };
				inputInstrument.ShowDialog();
				switch (inputInstrument.InputInstrumentResult)
				{
					case InputInstrumentWindow.InputInstrumentResults.Apply:
						Instrument newInstrument = new Instrument(t, inputInstrument.InstrumentName);
						_instruments.Add(newInstrument);
						_instrumentSequence.Add(newInstrument);
						result = InputValueResults.Apply;
						break;

					case InputInstrumentWindow.InputInstrumentResults.Ignore:
						_instrumentSequence.Add(new Instrument(t, string.Empty));
						result = InputValueResults.Apply;
						break;

					case InputInstrumentWindow.InputInstrumentResults.IgnoreAll:
						_ignoreAllUnknownInstruments = true;
						Instrument curInstrument = new Instrument(t, inputInstrument.InstrumentName);
						_instruments.Add(curInstrument);
						_instrumentSequence.Add(curInstrument);
						result = InputValueResults.Apply;
						break;

					case InputInstrumentWindow.InputInstrumentResults.Abort:
						result = InputValueResults.Abort;
						break;
				}
			}));

			return result;
		}


		public void ProcessFinished(bool success, string message)
		{
			_fvals.Clear();

			Dispatcher.Invoke(new Action(() =>
			{
				if (success)
				{
					txtStatus.Text = "Обработка файла завершена";

					if (_processSettings.ShowInstrumentsSequence)
					{
						if (_instrumentSequence.Count > 0)
						{
							InstrumentSequenceWindow sequence = new InstrumentSequenceWindow(_instrumentSequence) { Owner = this };
							sequence.ShowDialog();
						}
					}
					Close();
				}
				else
				{
					txtStatus.Text = message;
				}

				IsEnabled = true;
			}));
		}


		private void ProcessWindow_Activated(object sender, EventArgs e)
		{
			Topmost = true;
			Thread.Sleep(10);
			Topmost = false;
			Focus();
		}
	}
}
