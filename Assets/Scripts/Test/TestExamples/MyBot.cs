using System.Collections.Generic;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.Test.TestExamples
{
    /// <summary>
    /// Any custom class made by a student.
    /// </summary>
    public class MyBot
    {
        public static int Army = 0;
        public static bool AttackPhase = false;
        public static IUnit HealMeetPointUnit { get; set; }
        public static Event MeetPointEvent { get; set; }
        public static IList<IUnit> CurrentEnemies = new List<IUnit>();
    }
}
