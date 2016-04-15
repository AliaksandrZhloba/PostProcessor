using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

using PostProcessor.Entity;


namespace PostProcessor.UserControls
{
	public class TrayMenu : ContextMenuStrip
	{
		public Action CloseClicked, SettingsClicked, ProcessClicked, MatherialsClicked, InstrumentsClicked;


		public TrayMenu(Settings settings)
			: base()
		{
			ToolStripMenuItem mClose = new ToolStripMenuItem("Выход", PostProcessor.Properties.Resources.CloseIcon);
			mClose.Click += delegate(object sender, EventArgs e) { CloseClicked(); };

			ToolStripMenuItem mSettings = new ToolStripMenuItem("Настройки", PostProcessor.Properties.Resources.SettingsIcon);
			mSettings.Click += delegate(object sender, EventArgs e) { SettingsClicked(); };

			ToolStripMenuItem mMatherials = new ToolStripMenuItem("Материалы", PostProcessor.Properties.Resources.SettingsIcon) { Enabled = false };
			mMatherials.Click += delegate(object sender, EventArgs e) { MatherialsClicked(); };

			ToolStripMenuItem mInstruments = new ToolStripMenuItem("Инструменты", PostProcessor.Properties.Resources.SettingsIcon) { Enabled = false };
			mInstruments.Click += delegate(object sender, EventArgs e) { InstrumentsClicked(); };

			ToolStripMenuItem mProcess = new ToolStripMenuItem("Обработать", PostProcessor.Properties.Resources.StartIcon);
			mProcess.Click += delegate(object sender, EventArgs e) { ProcessClicked(); };

			Items.Add(mClose);
			Items.Add(new ToolStripSeparator());
			Items.Add(mInstruments);
			Items.Add(mMatherials);
			Items.Add(mSettings);
			Items.Add(new ToolStripSeparator());
			Items.Add(mProcess);
		}
	}
}
