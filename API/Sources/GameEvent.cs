using System;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game
{
    [Serializable]
    public class GameEvent
    {
        /// <summary>
        /// Returns true if the GameEvent was not unregistered yet.
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Type of the GameEvent that has to occur in the Game in order for the scheduled function to be run.
        /// </summary>
        public GameEventType Type { get; private set; }

        private GameEvent()
        {
        }

        public void RunFunction(Arguments arguments)
        {
            throw new NotImplementedException();
        }

        public static GameEvent Register(GameEventType gameEventType, Action<Arguments> function)
        {
            throw new NotImplementedException();
        }

        public void UnregisterEvent()
        {
            throw new NotImplementedException();
        }
    }
}
