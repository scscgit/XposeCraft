using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.App.TestRunner
{
	class Log
	{
		static Form1 main = Program.MainForm;

		public static void i(object context, string message)
		{
			main.Log(context, message);
		}

		public static void e(object context, string message)
		{
			main.Log(context, "---------------------------");
			main.Log(context, "ERROR!!! " + message);
			main.Log(context, "---------------------------");
		}
	}
}
