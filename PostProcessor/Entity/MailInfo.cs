using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PostProcessor.Entity
{
	public class MailInfo
	{
		public string SmtpClient
		{
			get;
			private set;
		}

		public int Port
		{
			get;
			private set;
		}

		public string MailAddress
		{
			get;
			private set;
		}
		
		public string UserName
		{
			get;
			private set;
		}
		
		public string UserPassword
		{
			get;
			private set;
		}


		public MailInfo(string smtpClient, int port, string mailAddress, string userName, string userPassword)
		{
			SmtpClient = smtpClient;
			Port = port;
			MailAddress = mailAddress;
			UserName = userName;
			UserPassword = userPassword;
		}
	}
}
