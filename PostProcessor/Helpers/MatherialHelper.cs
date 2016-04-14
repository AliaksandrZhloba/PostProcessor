using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml;
using System.IO;

using PostProcessor.Entity;


namespace PostProcessor.Helpers
{
	public static class MatherialHelper
	{
		public static BindingList<Matherial> LoadMatherials(string path)
		{
			BindingList<Matherial> matherials = new BindingList<Matherial>();


			using (XmlTextReader reader = new XmlTextReader(path))
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if ((string.Compare(reader.Name, "matherial", true) == 0))
						{
							string name = null, s_speed = null, s_fxy = null, s_fz = null;

							while (reader.MoveToNextAttribute())
							{
								if (string.Compare(reader.Name, "Name", true) == 0)
								{
									name = reader.Value;
								}
								else if (string.Compare(reader.Name, "Speed", true) == 0)
								{
									s_speed = reader.Value;
								}
								else if (string.Compare(reader.Name, "Fxy", true) == 0)
								{
									s_fxy = reader.Value;
								}
								else if (string.Compare(reader.Name, "Fz", true) == 0)
								{
									s_fz = reader.Value;
								}
							}

							int speed = 0, fxy = 0, fz = 0;
							if (int.TryParse(s_speed, out speed) && int.TryParse(s_fxy, out fxy) && int.TryParse(s_fz, out fz))
							{
								matherials.Add(new Matherial(name, speed, fxy, fz));
							}
						}
					}
				}
			}

			return matherials;
		}

		public static void SaveMatherials(string path, BindingList<Matherial> matherials)
		{
			if (File.Exists(path))
			{
				string bakfile = path + ".bak";
				if (File.Exists(bakfile))
				{
					File.Delete(bakfile);
				}
				File.Move(path, path + ".bak");
			}

			using (XmlTextWriter writer = new XmlTextWriter(path, Encoding.Unicode))
			{
				writer.WriteStartDocument();
				writer.WriteWhitespace(Environment.NewLine);
				writer.WriteStartElement("matherials");
				writer.WriteWhitespace(Environment.NewLine);

				foreach (Matherial matherial in matherials)
				{
					writer.WriteStartElement("matherial");
					writer.WriteAttributeString("Name", matherial.Name);
					writer.WriteAttributeString("Speed", matherial.Speed.ToString());
					writer.WriteAttributeString("Fxy", matherial.Fxy.ToString());
					writer.WriteAttributeString("Fz", matherial.Fz.ToString());

					writer.WriteEndElement();
					writer.WriteWhitespace(Environment.NewLine);
				}

				writer.WriteEndDocument();
			}
		}


		public static BindingList<Matherial> Clone(BindingList<Matherial> matherials)
		{
			BindingList<Matherial> copy = new BindingList<Matherial>();
			foreach (Matherial item in matherials)
			{
				copy.Add(item.Clone());
			}

			return copy;
		}
	}
}
