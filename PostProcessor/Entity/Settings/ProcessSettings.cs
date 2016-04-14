using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace PostProcessor.Entity
{
	public class ProcessSettings : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		public string FilePath
		{
			get;
			set;
		}

		public string Drive
		{
			get;
			set;
		}

		public bool ControlVC
		{
			get;
			set;
		}

		public bool SendToUSB
		{
			get;
			set;
		}


		private string _usblabel;

		public string USBLabel
		{
			get { return _usblabel; }
			set
			{
				_usblabel = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("USBLabel"));
				}
			}
		}

		public bool ClearUSB
		{
			get;
			set;
		}

		public bool GotoXYZ
		{
			get;
			set;
		}


		public uint XEnd
		{
			get;
			set;
		}

		public uint YEnd
		{
			get;
			set;
		}

		public uint ZEnd
		{
			get;
			set;
		}

		public bool ShowInstrumentsSequence
		{
			get;
			set;
		}


		public Matherial SourceMatherial
		{
			get;
			set;
		}


		public Matherial CurrentMatherial
		{
			get;
			set;
		}
	}
}
