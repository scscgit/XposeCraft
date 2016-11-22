using System;
using System.Collections.Generic;

namespace XposeCraft.GameInternal
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

		public static void CreateBuilding(double delay, GameTimer.TimedAction action)
		{
			GameTimer.Schedule(delay, () =>
			{
				action();

			});
		}
	}
}
