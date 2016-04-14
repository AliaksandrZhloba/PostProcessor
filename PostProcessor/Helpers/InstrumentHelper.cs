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
	public static class InstrumentHelper
	{
		public static BindingList<Instrument> LoadInstruments(string path)
		{
			BindingList<Instrument> instruments = new BindingList<Instrument>();

			using (XmlTextReader reader = new XmlTextReader(path))
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if ((string.Compare(reader.Name, "instrument", true) == 0))
						{
							string name = null, s_num = null;
							while (reader.MoveToNextAttribute())
							{
								if (string.Compare(reader.Name, "Name", true) == 0)
								{
									name = reader.Value;
								}
								else if (string.Compare(reader.Name, "Num", true) == 0)
								{
									s_num = reader.Value;
								}
							}

							int num = 0;
							if (int.TryParse(s_num, out num))
							{
								instruments.Add(new Instrument(num, name));
							}
						}
					}
				}
			}

			return instruments;
		}

		public static BindingList<Instrument> Clone(BindingList<Instrument> instruments)
		{
			BindingList<Instrument> copy = new BindingList<Instrument>();
			foreach (Instrument item in instruments)
			{
				copy.Add(item.Clone());
			}

			return copy;
		}

		public static void SaveInstruments(string path, BindingList<Instrument> instruments)
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
				writer.WriteStartElement("instruments");
				writer.WriteWhitespace(Environment.NewLine);

				foreach (Instrument instrument in instruments)
				{
					writer.WriteStartElement("instrument");
					writer.WriteAttributeString("Num", instrument.Num.ToString());
					writer.WriteAttributeString("Name", instrument.Name);

					writer.WriteEndElement();
					writer.WriteWhitespace(Environment.NewLine);
				}

				writer.WriteEndDocument();
			}
		}
	}
}
