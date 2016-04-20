using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using PostProcessor.Helpers;


namespace PostProcessor.Entity
{
	public class Processor
	{
		private const string PROCESSED = @"'processed";
		private const string VALID_PROCESSOR_TYPE = @"'MLC POST";

		public Action<bool, string> Finished;


		public delegate InputValueResults GetFValueDelegate(int f, int lnum, string[] lines, out int rez);
		public GetFValueDelegate GetFValue;

		
		public delegate InputValueResults ChangeInstrumentDetectedDelegate(int t, int lnum, string[] lines);
		public ChangeInstrumentDetectedDelegate ChangeInstrumentDetected;


		public bool IsRunning
		{
			get;
			private set;
		}


		public void Run(ProcessSettings settings)
		{
			IsRunning = true;

			string[] srcLines;
			try
			{
				srcLines = File.ReadAllLines(settings.FilePath);
			}
			catch (Exception)
			{
				Finished(false, "Ошибка чтения файла");
				IsRunning = false;
				return;
			}

			if (IsComment(srcLines[0]))
			{
				Finished(false, "Файл уже обработан");
				IsRunning = false;
				return;
			}

			if (!IsValidProcessorType(srcLines[0]))
			{
				Finished(false, "Исходный файл не обработан требуемым процессором");
				IsRunning = false;
				return;
			}

			int tn = 0;
			int tn_prev = -1;

			string fz = string.Format("F{0}", settings.CurrentMatherial.Fz);
			string fxy = string.Format("F{0}", settings.CurrentMatherial.Fxy);

			List<string> postLines = new List<string>();
			postLines.Add(PROCESSED);

			for (int lnum = 0; lnum < srcLines.Length; lnum++)
			{
				string line = srcLines[lnum];
				if (line == "M3")
				{
					postLines.Add(line + string.Format(" S{0}", settings.CurrentMatherial.Speed));
				}
				else if (TryParseTxM6(line, ref tn))
				{
					if (tn == tn_prev)
					{
						postLines.Add(line);
					}
					else
					{
						if (tn_prev == -1)
						{
							postLines.Add(line);
							if (settings.ControlVC)
							{
								postLines.Add("M8");
							}
							postLines.Add(string.Format("M3 S{0}", settings.CurrentMatherial.Speed));
						}
						else
						{
							//postLines.Add("G0 Y0.000 Z50.000");
							postLines.Add("G53 G0 Y0.000 Z0.000");
							//postLines.Add("M5");
							if (settings.ControlVC)
							{
								postLines.Add("M9");
							}
							postLines.Add(line);
							if (settings.ControlVC)
							{
								postLines.Add("M8");
							}
							postLines.Add(string.Format("M3 S{0}", settings.CurrentMatherial.Speed));
						}

						tn_prev = tn;
						if (settings.ShowInstrumentsSequence)
						{
							InputValueResults result = ChangeInstrumentDetected(tn, lnum, srcLines);
							if (result == InputValueResults.Abort)
							{
								Finished(false, "Выполнение прервано");
								IsRunning = false;
								return;
							}
						}
					}
				}
				else
				{
					if (line.StartsWith("G"))
					{
						string nextLine = srcLines[lnum + 1];
						bool lastLineOfFrame = (lnum < srcLines.Length - 1) && (!nextLine.StartsWith("G"));
						if (!lastLineOfFrame)
						{
							string tmp = string.Empty;
							string[] parts = line.Split(' ');
							for (int pnum = 0; pnum < parts.Length; pnum++)
							{
								string part = parts[pnum];
								if (part.StartsWith("F"))
								{
									int f = 0;
									if (int.TryParse(part.Remove(0, 1), out f))
									{
										if (f == 300)
										{
											part = string.Format("F{0}", settings.CurrentMatherial.Fz);
										}
										else if (f == 600)
										{
											part = string.Format("F{0}", settings.CurrentMatherial.Fxy);
										}
										else
										{
											int newFValue;
											InputValueResults result = GetFValue(f, lnum, srcLines, out newFValue);
											if (result == InputValueResults.Abort)
											{
												Finished(false, "Выполнение прервано");
												IsRunning = false;
												return;
											}
											part = string.Format("F{0}", newFValue);
										}
									}
								}

								tmp = tmp + part + " ";
							}
							postLines.Add(tmp);
						}
					}
					else
					{
						postLines.Add(line);
					}
				}
			}

			if (settings.GotoXYZ)
			{
				postLines.Insert(postLines.Count - 3, string.Format("G53 G0 Z0", settings.ZEnd));
				postLines.Insert(postLines.Count - 3, string.Format("G53 G0 X{0} Y{1}", settings.XEnd, settings.YEnd));
			}


			FileWatcher.Pause();
			try
			{
				if (settings.SendToUSB)
				{
					if (settings.ClearUSB)
					{
						LogHelper.GetLogger().Trace("Clearing USB...");
						DirectoryInfo di = new DirectoryInfo(settings.Drive);
						foreach (FileInfo file in di.GetFiles())
						{
							LogHelper.GetLogger().Trace("Deleting file ({0})", file.FullName);
							file.Delete();
							LogHelper.GetLogger().Trace("File deleted  ({0}).", file.FullName);
						}
						foreach (DirectoryInfo dir in di.GetDirectories())
						{
							LogHelper.GetLogger().Trace("Deleting directory ({0})", dir.FullName);
							dir.Delete(true);
							LogHelper.GetLogger().Trace("Directory deleted  ({0}).", dir.FullName);
						}
						LogHelper.GetLogger().Trace("USB Cleared.");
					}

					string fname = Path.GetFileName(settings.FilePath);
					string usbPath = Path.Combine(settings.Drive, fname);

					LogHelper.GetLogger().Trace("Writing file ({0})", usbPath);
					File.WriteAllLines(usbPath, postLines);
					LogHelper.GetLogger().Trace("File has been written ({0}).", usbPath);
				}

				string bakfile = settings.FilePath + ".bak";
				if (File.Exists(bakfile))
				{
					LogHelper.GetLogger().Trace("Deleting .bak file ({0}).", bakfile);
					File.Delete(bakfile);
					LogHelper.GetLogger().Trace(".bak file deleted ({0}).", bakfile);
				}

				LogHelper.GetLogger().Trace("Saving file as .bak file ({0}, {1}).", settings.FilePath, bakfile);
				File.Move(settings.FilePath, bakfile);
				LogHelper.GetLogger().Trace("File saved as .bak file ({0}, {1}).", settings.FilePath, bakfile);

				LogHelper.GetLogger().Trace("Writing file ({0})", settings.FilePath);
				File.WriteAllLines(settings.FilePath, postLines);
				LogHelper.GetLogger().Trace("File has been written ({0}).", settings.FilePath);
			}
			catch (Exception)
			{
				IsRunning = false;
				Finished(false, "Ошибка записи файла");
				return;
			}
			finally
			{
				FileWatcher.Continue();
			}

			IsRunning = false;
			Finished(true, null);
		}


		private bool IsValidProcessorType(string line)
		{
			return line.StartsWith(VALID_PROCESSOR_TYPE);
		}

		private bool IsComment(string line)
		{
			return line.StartsWith(PROCESSED);
		}


		private bool TryParseTxM6(string line, ref int tn)
		{
			if (line.StartsWith("T"))
			{
				string[] parts = line.Split(' ');
				string tmp = parts[0].Remove(0, 1);
				return int.TryParse(tmp, out tn);
			}
			else
			{
				return false;
			}
		}
	}
}
