using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XposeCraft_UI_API_Prototype_Test.Test;

namespace XposeCraft_UI_API_Prototype_Test.TestRunner
{
	class TestRunner
	{
		Form1 main = Program.MainForm;

		protected void Sleep(int milliseconds)
		{
			System.Threading.Thread.Sleep(milliseconds);
		}

		protected void Log(object context, string text)
		{
			main.Log(context, text);
		}

		protected string SuccessString(bool success)
		{
			return success ? "finished successfully" : "failed";
		}

		public void RunTests()
		{
			bool result;
			GameInternal.RegisteredEvents.Initialize();

			Log(null, "----------------------------------");
			Log(null, ">>  Starting a new Test Round.  <<");
			Log(null, "----------------------------------");

			Log(this, "Starting Economy Stage");
			var first = new Economy();
			result = Economy(first);
			Log(first, "End of Economy Stage: " + SuccessString(result));

			Log(this, "Starting Building Stage");
			var second = new Building();
			result = Building(second);
			Log(first, "End of Building Stage: " + SuccessString(result));


			Log(this, "Starting Battle Stage");
			var third = new Battle();
			result = Battle(third);

			Log(first, "End of Battle Stage: " + SuccessString(result));

			Log(null, "");
			Log(null, ">>   End of a Test Round.   <<");
			Log(null, "");
		}

		bool Economy(Economy economy)
		{

			return true;
		}

		bool Building(Building building)
		{

			return true;
		}

		bool Battle(Battle battle)
		{

			return true;
		}
	}
}
