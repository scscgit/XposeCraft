using System;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game
{
    [Serializable]
    public class GameEvent
    {
        [Serializable]
        public class ArgumentsEvent : UnityEvent<Arguments>
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

        /// <summary>
        /// Scheduled function to be executed when the GameEvent occurs in the Game.
        /// </summary>
        public UnityAction<Arguments> FunctionWithArguments { get; private set; }

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

        public void RunFunction(Arguments arguments)
        {
            _serializedEvent.Invoke(arguments);
        }

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
