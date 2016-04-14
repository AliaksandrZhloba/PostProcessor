using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PostProcessor.Entity
{
	public class Instrument
	{
		public int Num
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public Instrument(int num, string name)
		{
			Num = num;
			Name = name;
		}


		public Instrument Clone()
		{
			return new Instrument(Num, Name);
		}
	}
}
