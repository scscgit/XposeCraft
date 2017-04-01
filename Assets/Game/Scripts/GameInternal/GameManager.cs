using UnityEngine;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Resources;
using XposeCraft.Game;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using EventType = XposeCraft.Game.Enums.EventType;

namespace XposeCraft.GameInternal
{
    public class GameManager : MonoBehaviour
    {
        public const string ScriptName = "Game Manager";

        public static GameManager Instance;

        public Player[] Players;
        public Object BaseCenterPrefab;
        public Object WorkerPrefab;
        public int StartingWorkers = 1;
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

        private void Start()
        {
            foreach (var player in Players)
            {
                Player.CurrentPlayer = player;
                Actor.Create<BaseCenter>(
                    (GameObject) Instantiate(
                        BaseCenterPrefab,
                        player.MyBase.Center.Location,
                        Quaternion.identity)
                );
                for (var workerIndex = 0; workerIndex < StartingWorkers; workerIndex++)
                {
                    Actor.Create<Worker>(
                        (GameObject) Instantiate(
                            WorkerPrefab,
                            ((BaseCenter) player.Buildings[0]).SpawnPosition.Location,
                            Quaternion.identity)
                    );
                }
            }
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
