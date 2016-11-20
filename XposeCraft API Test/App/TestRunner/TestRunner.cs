using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;
using XposeCraft_UI_API_Prototype_Test.Game.Enum;
using XposeCraft_UI_API_Prototype_Test.GameInternal;
using XposeCraft_UI_API_Prototype_Test.Test;

namespace XposeCraft_UI_API_Prototype_Test.App.TestRunner
{
	class TestRunner
	{
		protected void Sleep(int milliseconds)
		{
			System.Threading.Thread.Sleep(milliseconds);
		}

		protected string SuccessString(bool success)
		{
			return success ? "finished successfully" : "failed";
		}

		public void RunTests()
		{
			bool result;
			RegisteredEvents.Initialize();
			Model.Instance.Units.Add(new Worker(PlaceType.NearBase));

			Log.i(null, "----------------------------------");
			Log.i(null, ">>  Starting a new Test Round.  <<");
			Log.i(null, "----------------------------------");

			Log.i(this, "Starting Economy Stage");
			var first = new Economy();
			result = Economy(first);
			Log.i(first, "End of Economy Stage: " + SuccessString(result));

			Log.i(this, "Starting Building Stage");
			var second = new Building();
			result = Building(second);
			Log.i(first, "End of Building Stage: " + SuccessString(result));


			Log.i(this, "Starting Battle Stage");
			var third = new Battle();
			result = Battle(third);

			Log.i(first, "End of Battle Stage: " + SuccessString(result));

			Log.i(null, "");
			Log.i(null, ">>   End of a Planning Phase.   <<");
			Log.i(null, "");

			var gameTimer = new GameTimer();
			gameTimer.RunGame(() =>
			{
				if (gameTimer.Cycle >= 500)
				{
					return true;
				}
				return false;
			});

			Log.i(null, "");
			Log.i(null, ">>   End of Game.   <<");
			Log.i(null, "");
		}

		bool Economy(Economy economy)
		{
			economy.EconomyStage();
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
