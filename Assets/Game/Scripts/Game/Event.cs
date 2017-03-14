using System;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game
{
    [Serializable]
    public class Event
    {
        /// <summary>
        /// Returns true if the Event was not unregistered yet.
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Type of the event that has to occur in the Game in order for the scheduled Function to be run.
        /// </summary>
        public EventType GameEvent { get; private set; }

        /// <summary>
        /// Scheduled function to be executed when the GameEvent occurs in the Game.
        /// </summary>
        public FunctionWithArguments Function { get; private set; }

        public delegate void FunctionWithArguments(Arguments args);

        private Event(EventType gameEvent, FunctionWithArguments function)
        {
            GameEvent = gameEvent;
            Function = function;
        }

        public static Event Register(EventType gameEvent, FunctionWithArguments function)
        {
            var registeredEvents = Player.CurrentPlayer.RegisteredEvents;

            // Dictionary key initialization if this is the first time for the GameEvent
            if (!registeredEvents.ContainsKey(gameEvent))
            {
                registeredEvents.Add(gameEvent, new Player.EventList());
            }

            // Registering the function
            var newEvent = new Event(gameEvent, function)
            {
                IsRegistered = true
            };
            registeredEvents[gameEvent].Add(newEvent);
            return newEvent;
        }

        public void UnregisterEvent()
        {
            if (IsRegistered)
            {
                IsRegistered = false;
                // TODO: this will probably have to be replaced by a loop that removes all unregistered events at once
                Player.CurrentPlayer.RegisteredEvents[GameEvent].Remove(this);
            }
        }
    }
}
