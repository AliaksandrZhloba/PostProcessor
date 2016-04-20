using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using PostProcessor.Entity;



namespace PostProcessor.Helpers
{
	public static class MailInfoHelper
	{
		public static MailInfo LoadMailInfo(string path)
		{
			using (XmlTextReader reader = new XmlTextReader(path))
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if ((string.Compare(reader.Name, "MailInfo", true) == 0))
						{
							string smtpClient = null, mailAddress = null, userName = null, userPassword = null, s_port = null;

							while (reader.MoveToNextAttribute())
							{
								if (string.Compare(reader.Name, "SmtpClient", true) == 0)
								{
									smtpClient = reader.Value;
								}
								else if (string.Compare(reader.Name, "MailAddress", true) == 0)
								{
									mailAddress = reader.Value;
								}
								else if (string.Compare(reader.Name, "UserName", true) == 0)
								{
									userName = reader.Value;
								}
								else if (string.Compare(reader.Name, "UserPassword", true) == 0)
								{
									userPassword = reader.Value;
								}
								else if (string.Compare(reader.Name, "Port", true) == 0)
								{
									s_port = reader.Value;
								}
							}

							int port = 0;
							if (int.TryParse(s_port, out port))
							{
								return new MailInfo(smtpClient, port, mailAddress, userName, userPassword);
							}
						}
					}
				}
			}

			return null;
		}
	}
}
