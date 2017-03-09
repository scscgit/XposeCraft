using System.Collections.Generic;

namespace XposeCraft.GameInternal
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
            Delay = delay;
            Action = action;
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

        public double Cycle { get; private set; }

        static IList<DelayActionPair> ScheduledActions = new List<DelayActionPair>();

        public static void Schedule(double delay, TimedAction action)
        {
            ScheduledActions.Add(new DelayActionPair(delay, action));
        }

        public void RunGame(EndConditionDelegate endCondition)
        {
            while (!endCondition())
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
