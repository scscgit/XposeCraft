using System;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game
{
    /// <summary>
    /// Event registered in the game system of events, triggering its anonymous function on <see cref="GameEventType"/>
    /// game event occurrence. This function receives an event description in the form of <see cref="Arguments"/>.
    /// </summary>
    [Serializable]
    public class GameEvent
    {
        [Serializable]
        private class ArgumentsEvent : UnityEvent<Arguments>
        {
        }

        /// <summary>
        /// Returns true if the GameEvent was not unregistered yet.
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Type of the GameEvent that has to occur in the Game in order for the scheduled function to be run.
        /// </summary>
        public GameEventType Type { get; private set; }

        private ArgumentsEvent _serializedEvent = new ArgumentsEvent();

        private GameEvent(GameEventType type, UnityAction<Arguments> function)
        {
            Type = type;
#if UNITY_EDITOR
            // When in the Editor, the listener is persisted during a hot-swap
            if (function.Target is ScriptableObject)
            {
                UnityEventTools.AddPersistentListener(_serializedEvent, function);
            }
            else
            {
                // Enemies of the GUI player will not trigger warnings
                if (GameManager.Instance.GuiPlayer == Player.CurrentPlayer)
                {
                    Log.w("Registered event to non-serialized function that will be lost on hot-swap");
                }
                _serializedEvent.AddListener(function);
            }
#else
            _serializedEvent.AddListener(function);
#endif
        }

        internal void RunFunction(Arguments arguments)
        {
            _serializedEvent.Invoke(arguments);
        }

        /// <summary>
        /// Registers a new instance in the game event system to wait for the <see cref="GameEventType"/> occurrence.
        /// </summary>
        /// <param name="gameEventType">Type of the game event to get triggered on.</param>
        /// <param name="function">Function to be executed on the event trigger, receiving an event description.</param>
        /// <returns>Registered event representation, which can be later used to unregister it.</returns>
        public static GameEvent Register(GameEventType gameEventType, UnityAction<Arguments> function)
        {
            var registeredEvents = Player.CurrentPlayer.RegisteredEvents;

            // Dictionary key initialization if this is the first time for the GameEvent
            if (!registeredEvents.ContainsKey(gameEventType))
            {
                registeredEvents.Add(gameEventType, new Player.EventList());
            }

            // Registering the function
            var newEvent = new GameEvent(gameEventType, function)
            {
                IsRegistered = true
            };
            registeredEvents[gameEventType].Add(newEvent);

            if (gameEventType == GameEventType.MineralsChanged)
            {
                Tutorial.Instance.EventMineralsChanged();
            }
            return newEvent;
        }

        /// <summary>
        /// Unregisters the registered GameEvent.
        /// </summary>
        public void UnregisterEvent()
        {
            if (IsRegistered)
            {
                IsRegistered = false;
                // TODO: this will probably have to be replaced by a loop that removes all unregistered events at once
                Player.CurrentPlayer.RegisteredEvents[Type].Remove(this);
            }
        }
    }
}
