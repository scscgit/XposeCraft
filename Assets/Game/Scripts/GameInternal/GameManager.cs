using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Resources;
using XposeCraft.Game;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;
using XposeCraft.GameInternal.Helpers;
using BuildingType = XposeCraft.Game.Enums.BuildingType;
using EventType = XposeCraft.Game.Enums.EventType;

namespace XposeCraft.GameInternal
{
    public class GameManager : MonoBehaviour
    {
        public const string ScriptName = "Game Manager";

        public static GameManager Instance;

        public Player[] Players;
        public GameObject BaseCenterProgressPrefab;
        public GameObject WorkerPrefab;
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
            List<Resource> resources = new List<Resource>();
            foreach (var resourceSource in FindObjectsOfType<ResourceSource>())
            {
                resources.Add(Resource.CreateResourceActor<Resource>(resourceSource.gameObject));
            }

            foreach (var player in Players)
            {
                // Spawn starting Bases and Workers
                Player.CurrentPlayer = player;
                Actor.Create<BaseCenter>(
                    BuildingPlacement.InstantiateProgressBuilding(
                            BuildingHelper.FindBuildingInFaction(BuildingType.BaseCenter, null),
                            BaseCenterProgressPrefab,
                            player.FactionIndex,
                            player.MyBase.Center,
                            Quaternion.identity,
                            UGrid.grids[UGrid.index],
                            Fog)
                        .GetComponent<BuildingController>()
                        .Place()
                        .gameObject);
                for (var workerIndex = 0; workerIndex < StartingWorkers; workerIndex++)
                {
                    Actor.Create<Worker>(
                        (GameObject) Instantiate(
                            WorkerPrefab,
                            // Workers will be spawned near the first Base
                            ((BaseCenter) player.Buildings[0]).SpawnPosition.Location,
                            Quaternion.identity)
                    );
                }

                // Initialize Resources to use the shared list
                player.Resources = resources;
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
