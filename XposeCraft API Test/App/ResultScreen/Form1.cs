using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using XposeCraft.App.TestRunner;

namespace XposeCraft.App.ResultScreen
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		//protected override void OnLoad(EventArgs e)
		//{
		//	base.OnLoad(e);
		//	CheckForIllegalCrossThreadCalls = false;
		//}

		static object ConcurrentTestPreventionLock = new object();
		void button1_Click(object sender, EventArgs e)
		{
			lock (ConcurrentTestPreventionLock)
			{
				new Thread(() =>
					this.BeginInvoke(
						new Action(() => new Runner().RunTests())
					)
				).Start();
			}
		}
	}
}
