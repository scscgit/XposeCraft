using System;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game
{
    /// <summary>
    /// Event registered in the game system of events, triggering its anonymous function on <see cref="GameEventType"/>
    /// game event occurrence. This function receives an event description in the form of <see cref="Arguments"/>.
    /// </summary>
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

        /// <summary>
        /// Registers a new instance in the game event system to wait for the <see cref="GameEventType"/> occurrence.
        /// </summary>
        /// <param name="gameEventType">Type of the game event to get triggered on.</param>
        /// <param name="function">Function to be executed on the event trigger, receiving an event description.</param>
        /// <returns>Registered event representation, which can be later used to unregister it.</returns>
        public static GameEvent Register(GameEventType gameEventType, Action<Arguments> function)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters the registered GameEvent.
        /// </summary>
        public void UnregisterEvent()
        {
            throw new NotImplementedException();
        }
    }
}
