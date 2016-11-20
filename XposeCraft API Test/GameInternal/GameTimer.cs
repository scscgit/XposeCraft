using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XposeCraft_UI_API_Prototype_Test.GameInternal
{
	/// <summary>
	/// Pair of actions and their delays for time calculation.
	/// </summary>
	class DelayActionPair
	{
		public DelayActionPair()
		{
		}

		public DelayActionPair(double delay, GameTimer.TimedAction action)
		{
			this.Delay = delay;
			this.Action = action;
		}

		public double Delay;
		public GameTimer.TimedAction Action;
	}

	/// <summary>
	/// System for handling time of game cycles and their atomicity.
	/// </summary>
	class GameTimer
	{
		public delegate bool EndConditionDelegate();

		public delegate void TimedAction();
		public double Cycle
		{
			get; private set;
		} = 0;

		static IList<DelayActionPair> ScheduledActions = new List<DelayActionPair>();

		public static void Schedule(double delay, TimedAction action)
		{
			ScheduledActions.Add(new DelayActionPair(delay, action));
		}

		public void RunGame(EndConditionDelegate EndCondition)
		{
			while (!EndCondition())
			{
				GoToNextCycle();
			}
		}

		protected void GoToNextCycle()
		{
			ExecuteScheduledActions();
			ExecuteGameProgress();
			Cycle++;
		}

		protected void ExecuteScheduledActions()
		{
			IList<DelayActionPair> toRemove = new List<DelayActionPair>();

			foreach (DelayActionPair pair in ScheduledActions)
			{
				pair.Delay--;
				if (pair.Delay <= 0)
				{
					pair.Action();
					toRemove.Add(pair);
				}
			}

			foreach (DelayActionPair old in toRemove)
			{
				ScheduledActions.Remove(old);
			}
		}

		protected void ExecuteGameProgress()
		{

		}
	}
}
