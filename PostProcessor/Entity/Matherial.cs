using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PostProcessor.Entity
{
	public class Matherial
	{
		public string Name
		{
			get;
			set;
		}

		public int Speed
		{
			get;
			set;
		}

		public int Fxy
		{
			get;
			set;
		}

		public int Fz
		{
			get;
			set;
		}


		public Matherial(string name, int speed, int fxy, int fz)
		{
			Name = name;
			Speed = speed;
			Fxy = fxy;
			Fz = fz;
		}


		public Matherial Clone()
		{
			return new Matherial(this.Name, this.Speed, this.Fxy, this.Fz);
		}


		public static string Serialize(Matherial matherial)
		{
			if (matherial == null)
			{
				return string.Empty;
			}

			return string.Format("{0} {1} {2} {3}", matherial.Name, matherial.Speed, matherial.Fxy, matherial.Fz);
		}

		public static Matherial Deserialize(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				string[] parts = str.Split(' ');
				if (parts.Length == 4)
				{
					int spd = 0, fxy = 0, fz = 0;
					if (int.TryParse(parts[1], out spd) && int.TryParse(parts[2], out fxy) && int.TryParse(parts[3], out fz))
					{
						return new Matherial(parts[0], spd, fxy, fz);
					}
				}
			}

			return null;
		}


		public static bool Equals(Matherial a, Matherial b)
		{
			return ((a.Name == b.Name) && (a.Speed == b.Speed) && (a.Fxy == b.Fxy) && (a.Fz == b.Fz));
		}
	}
}
