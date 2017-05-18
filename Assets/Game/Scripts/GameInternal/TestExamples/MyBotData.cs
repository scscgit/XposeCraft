using System.Collections.Generic;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Units;

namespace XposeCraft.GameInternal.TestExamples
{
    /// <summary>
    /// Additional serialized place to store custom data and references.
    /// </summary>
    internal class MyBotData : BotScript
    {
        public int Army;
        public Unit HealMeetPointUnit { get; set; }
        public GameEvent MeetPointEvent { get; set; }
        public List<IUnit> CurrentEnemies = new List<IUnit>();
    }
}
