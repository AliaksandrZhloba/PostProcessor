using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.IO.Pipes;
using System.Reflection;
using System.Globalization;

using Microsoft.Shell;

using PostProcessor.UserControls;
using PostProcessor.Helpers;
using PostProcessor.Entity;


namespace PostProcessor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application, ISingleInstanceApp
	{
		private const string Unique = "Post Processor App";
		private PostProcessorApp _ppApp;


		[STAThread]
		public static void Main()
		{
			if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
			{
				var application = new App();

				application.InitializeComponent();
				application.Run();

				// Allow single instance code to perform cleanup operations
				SingleInstance<App>.Cleanup();
			}
		}


		public bool SignalExternalCommandLineArgs(IList<string> args)
		{
			// handle command line arguments of second instance
			if (args.Count == 2 && !_ppApp.IsBusy)
			{
				string file = args[1];
				_ppApp.ProcessFile(file);
			}

			return true;
		}


		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			_ppApp = new PostProcessorApp();
			_ppApp.Run();

			string[] args = Environment.GetCommandLineArgs();

			if (args.Length == 2)
			{
				string file = args[1];
				_ppApp.ProcessFile(file);
			}
		}
	}
}
