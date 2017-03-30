using UnityEngine;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Resources;
using XposeCraft.Game;
using EventType = XposeCraft.Game.Enums.EventType;

namespace XposeCraft.GameInternal
{
    public class GameManager : MonoBehaviour
    {
        public const string ScriptName = "Game Manager";

        public static GameManager Instance;

        public Player[] Players;
        private object _firedEventLock;

        public UGrid UGrid { get; private set; }
        public Fog Fog { get; private set; }
        public ResourceManager ResourceManager { get; private set; }

        private void OnDrawGizmos()
        {
            if (name != ScriptName)
            {
                name = ScriptName;
            }
        }

        private void Awake()
        {
            Instance = this;
            _firedEventLock = new object();
            UGrid = GameObject.Find(UGrid.ScriptName).GetComponent<UGrid>();
            Fog = GameObject.Find(Fog.ScriptName).GetComponent<Fog>();
            ResourceManager = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
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
