using UnityEngine;
using XposeCraft.Game;
using EventType = XposeCraft.Game.Enums.EventType;

namespace XposeCraft.GameInternal
{
    public class GameManager : MonoBehaviour
    {
        public const string ScriptName = "Game Manager";

        public Player[] Players;
        private object _firedEventLock;

        private void OnDrawGizmos()
        {
            if (name != ScriptName)
            {
                name = ScriptName;
            }
        }

        private void Awake()
        {
            _firedEventLock = new object();
        }

        private void Update()
        {
        }

        public void FiredEvent(EventType eventType, Arguments args)
        {
            lock (_firedEventLock)
            {
                foreach (var player in Players)
                {
                    foreach (var registeredEvent in player.RegisteredEvents[eventType])
                    {
                        registeredEvent.Function(args);
                    }
                }
            }
        }
    }
}
