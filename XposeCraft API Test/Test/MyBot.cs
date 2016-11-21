using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XposeCraft_UI_API_Prototype_Test.Game.Actors.Units;

namespace XposeCraft_UI_API_Prototype_Test.Test
{
	/// <summary>
	/// Any custom class made by a student.
	/// </summary>
	public class MyBot
	{
		public static IUnit HealMeetPointUnit { get; set; }
		public static IList<IUnit> CurrentEnemies = new List<IUnit>();
	}
}
