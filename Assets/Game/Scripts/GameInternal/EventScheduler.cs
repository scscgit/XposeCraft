namespace XposeCraft.GameInternal
{
    class EventScheduler
    {
        /// <summary>
        /// Instance of the event scheduler.
        /// </summary>
        private static EventScheduler _instance;

        public static EventScheduler Instance
        {
            get { return _instance ?? (_instance = new EventScheduler()); }
        }

        public static void CreateUnit(double delay, GameTimer.TimedAction action)
        {
            GameTimer.Schedule(delay, action);
        }

        public static void CreateBuilding(double delay, GameTimer.TimedAction action)
        {
            GameTimer.Schedule(delay, action);
        }
    }
}
