using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XposeCraft_UI_API_Prototype_Test
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
						new Action(() => new TestRunner.TestRunner().RunTests())
					)
				).Start();
			}
		}
	}
}
