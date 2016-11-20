using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.GameInternal
{
	class EventScheduler
	{
		/// <summary>
		/// Instance of the event scheduler
		/// </summary>
		public static EventScheduler Instance
		{
			get;
		} = new EventScheduler();



		public static void CreateUnit(double delay, GameTimer.TimedAction action)
		{
			GameTimer.Schedule(delay, () =>
			{
				action();

			});
		}
	}
}
