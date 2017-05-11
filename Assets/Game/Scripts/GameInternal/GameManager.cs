using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTest;
using XposeCraft.Collections;
using XposeCraft.Core.Faction;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.Core.Grids;
using XposeCraft.Core.Required;
using XposeCraft.Core.Resources;
using XposeCraft.Game;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal.Helpers;
using Building = XposeCraft.Game.Actors.Buildings.Building;
using Event = XposeCraft.Game.Event;
using EventType = XposeCraft.Game.Enums.EventType;
using Unit = XposeCraft.Game.Actors.Units.Unit;

namespace XposeCraft.GameInternal
{
    public class GameManager : MonoBehaviour
    {
        [Serializable]
        public class ActorLookupDictionary : SerializableDictionary2<GameObject, Actor>
        {
        }

        public const string ScriptName = "Game Manager";

        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                return _instance
                       ?? (_instance = GameObject.Find(ScriptName).GetComponent<GameManager>().Initialize());
            }
        }

        public Player[] Players;
        public GameObject BaseCenterProgressPrefab;
        public GameObject WorkerPrefab;
        public int StartingWorkers = 1;
        public bool Debug;
        public bool DisplayAllHealthBars;
        public bool DisplayOnlyDamagedHealthBars;
        public Log.LogLevel LogLevel = Log.LogLevel.Debug;

        private object _firedEventLock;

        public object FiredEventLock
        {
            get { return _firedEventLock ?? (_firedEventLock = new object()); }
        }

        public Terrain Terrain { get; private set; }
        public UGrid UGrid { get; private set; }
        public Fog Fog { get; private set; }
        public AStarManager AStarManager { get; private set; }
        public ResourceManager ResourceManager { get; private set; }
        public Faction[] Factions { get; private set; }

        public Grid Grid
        {
            get { return UGrid.grids[UGrid.index]; }
        }

        public ActorLookupDictionary ActorLookup { get; private set; }

        private void OnDrawGizmos()
        {
            if (name != ScriptName)
            {
                name = ScriptName;
            }
        }

        private void Awake()
        {
            Initialize();
        }

        private GameManager Initialize()
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = this;
            Terrain = GameObject.Find("BasicTerrain").GetComponent<Terrain>();
            UGrid = GameObject.Find(UGrid.ScriptName).GetComponent<UGrid>();
            Fog = GameObject.Find(Fog.ScriptName).GetComponent<Fog>();
            AStarManager = GameObject.Find(AStarManager.ScriptName).GetComponent<AStarManager>();
            ResourceManager = GameObject.Find("Player Manager").GetComponent<ResourceManager>();
            var factionList = GameObject.Find("Faction Manager")
                .GetComponent<FactionManager>()
                .FactionList;
            Factions = new Faction[factionList.Length];
            for (var factionIndex = 0; factionIndex < factionList.Length; factionIndex++)
            {
                Factions[factionIndex] = factionList[factionIndex].GetComponent<Faction>();
            }
            ActorLookup = new ActorLookupDictionary();
            return this;
        }

        private void OnEnable()
        {
            // Initialize singleton on wake up after hot-swap, because lazy initialization from other thread is illegal
            _instance = this;
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
                    BuildingHelper.InstantiateProgressBuilding(
                            BuildingHelper.FindBuildingInFaction(BuildingType.BaseCenter, null),
                            BaseCenterProgressPrefab,
                            player.FactionIndex,
                            player.MyBase.Center,
                            Quaternion.identity)
                        .Place()
                        .gameObject
                    , player);
                for (var workerIndex = 0; workerIndex < StartingWorkers; workerIndex++)
                {
                    Actor.Create<Worker>(
                        UnitHelper.InstantiateUnit(
                            WorkerPrefab,
                            // Workers will be spawned near the first Base
                            PositionHelper.PositionToLocation(((BaseCenter) player.Buildings[0]).SpawnPosition),
                            player.FactionIndex),
                        player);
                }

                // Initialize Resources to use the shared list
                player.Resources = resources;

                // Initialize (single) enemy player for each of them
                var enemyFactionIndex = player.Faction.EnemyFactionIndexes()[0];
                foreach (var enemyPlayer in Players)
                {
                    if (enemyPlayer.FactionIndex != enemyFactionIndex)
                    {
                        continue;
                    }
                    player.EnemyBase = enemyPlayer.MyBase;
                    break;
                }
            }

            // Start the Test (build workaround)
            StartCoroutine(RunAutomationTest(SceneManager.LoadSceneAsync("AutomationTest", LoadSceneMode.Additive)));
        }

        [Obsolete]
        private IEnumerator LoadAutomationTest(AsyncOperation waitFor)
        {
            while (!waitFor.isDone)
            {
                yield return null;
            }
            StartCoroutine(RunAutomationTest(SceneManager.LoadSceneAsync("AutomationTest", LoadSceneMode.Additive)));
        }

        private IEnumerator RunAutomationTest(AsyncOperation waitFor)
        {
            while (!waitFor.isDone)
            {
                yield return null;
            }
            var allTestComponents = SceneManager.GetSceneByName("AutomationTest")
                .GetRootGameObjects()
                .ToList()
                .ConvertAll(obj => obj.GetComponent<TestComponent>())
                .FindAll(component => component != null);

            var dynamicTests = allTestComponents.Where(t => t.dynamic).ToList();
            var dynamicTestsToRun = dynamicTests.Select(c => c.dynamicTypeName).ToList();
            allTestComponents.RemoveAll(dynamicTests.Contains);

            TestComponent.DisableAllTests();
            var testRunner = TestRunner.GetTestRunner();
            testRunner.InitRunner(allTestComponents, dynamicTestsToRun);
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
                Log.d("Event " + eventType + " fired with "
                      + (!player.RegisteredEvents.ContainsKey(eventType)
                          ? "no"
                          : player.RegisteredEvents[eventType].Count.ToString()) + " listeners");
                if (!player.RegisteredEvents.ContainsKey(eventType))
                {
                    return;
                }
                // Copy current events before iterating over them
                var events = new List<Event>(player.RegisteredEvents[eventType]);
                foreach (var registeredEvent in events)
                {
                    // If the function is missing, most likely for hot-swap reasons, it will get unregistered
                    if (registeredEvent.Function == null)
                    {
                        Log.d(this, "Registered event " + registeredEvent.GameEvent + " didn't have any function");
                        registeredEvent.UnregisterEvent();
                        continue;
                    }
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

        public Player FindPlayerOfActor(Actor actor)
        {
            foreach (var player in Players)
            {
                var unit = actor as Unit;
                if (unit != null && player.Units.Contains(unit))
                {
                    return player;
                }
                var building = actor as Building;
                if (building != null && player.Buildings.Contains(building))
                {
                    return player;
                }
            }
            throw new Exception("Actor is not located in model of any Player");
        }
    }
}
