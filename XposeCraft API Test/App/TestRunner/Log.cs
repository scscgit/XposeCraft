using XposeCraft.App.ResultScreen;

namespace XposeCraft.App.TestRunner
{
	class Log
	{
		static Form1 Main = Program.MainForm;

		public static void i(object context, string message)
		{
			Main.Log(context, message);
		}

		public static void e(object context, string message)
		{
			Main.Log(context, "---------------------------");
			Main.Log(context, "ERROR!!! " + message);
			Main.Log(context, "---------------------------");
		}
	}
}
