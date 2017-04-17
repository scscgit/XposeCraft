using System;
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
using Event = XposeCraft.Game.Event;
using EventType = XposeCraft.Game.Enums.EventType;

namespace XposeCraft.GameInternal
{
    public class GameManager : MonoBehaviour
    {
        public const string ScriptName = "Game Manager";

        private static GameManager _instance;

        public static GameManager Instance
        {
            get { return _instance ?? (_instance = GameObject.Find(ScriptName).GetComponent<GameManager>()); }
        }

        public Player[] Players;
        public GameObject BaseCenterProgressPrefab;
        public GameObject WorkerPrefab;
        public int StartingWorkers = 1;
        public bool Debug;
        public bool DisplayAllHealthBars;
        public bool DisplayOnlyDamagedHealthBars;

        private object _firedEventLock;

        public object FiredEventLock
        {
            get { return _firedEventLock ?? (_firedEventLock = new object()); }
        }

        public UGrid UGrid { get; private set; }
        public Fog Fog { get; private set; }
        public ResourceManager ResourceManager { get; private set; }

        public Grid Grid
        {
            get { return UGrid.grids[UGrid.index]; }
        }

        private void OnDrawGizmos()
        {
            if (name != ScriptName)
            {
                name = ScriptName;
            }
        }

        private void Awake()
        {
            _instance = this;
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
                            Quaternion.identity)
                        .GetComponent<BuildingController>()
                        .Place()
                        .gameObject
                    , player);
                for (var workerIndex = 0; workerIndex < StartingWorkers; workerIndex++)
                {
                    Actor.Create<Worker>(
                        (GameObject) Instantiate(
                            WorkerPrefab,
                            // Workers will be spawned near the first Base
                            ((BaseCenter) player.Buildings[0]).SpawnPosition.Location,
                            Quaternion.identity)
                        , player);
                }

                // Initialize Resources to use the shared list
                player.Resources = resources;
            }
        }

        private void Update()
        {
        }

        /// <summary>
        /// Fires an Event for the Player to run his registered actions.
        /// TODO: if Arguments get reused between multiple players, do a deep clone.
        /// </summary>
        /// <param name="player">Context of the Model to be used.</param>
        /// <param name="eventType">Game Event that is fired.</param>
        /// <param name="args">Arguments to be used.</param>
        public void FiredEvent(Player player, EventType eventType, Arguments args)
        {
            lock (FiredEventLock)
            {
                if (player == null)
                {
                    throw new Exception("Attempt to trigger Event without Player context. " +
                                        "Maybe the Controller Script wasn't initialized by creating Actor?");
                }
                Player.CurrentPlayer = player;
                if (!player.RegisteredEvents.ContainsKey(eventType))
                {
                    return;
                }
                // Copy current events before iterating over them
                var events = new List<Event>(player.RegisteredEvents[eventType]);
                foreach (var registeredEvent in events)
                {
                    registeredEvent.Function(new Arguments(args, registeredEvent));
                }
            }
        }

        [Obsolete]
        public void FiredEventForAllPlayers(EventType eventType, Arguments args)
        {
            foreach (var player in Players)
            {
                FiredEvent(player, eventType, args);
            }
        }
    }
}
