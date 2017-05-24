using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using XposeCraft.BotScripts;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;

namespace XposeCraft.GameInternal
{
    public class GameTestRunner : MonoBehaviour
    {
        public const string ScriptName = "Game Test";

        public static bool Passed { get; set; }
        public static bool Failed { get; set; }
        internal bool WasStarted { get; private set; }
        private bool _shouldRun = true;

        private EconomyTest _playerEconomyTest;
        private BuildingTest _playerBuildingTest;
        private BattleTest _playerBattleTest;
        private MyBotData _playerBotData;

        private TestExamples.EconomyTest _enemyPlayerEconomyTest;
        private TestExamples.BuildingTest _enemyPlayerBuildingTest;
        private TestExamples.BattleTest _enemyPlayerBattleTest;
        private TestExamples.MyBotData _enemyPlayerBotData;

        protected void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        protected IEnumerator RunAfterSeconds(int seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
        }

        private void OnDrawGizmos()
        {
            if (name != ScriptName)
            {
                name = ScriptName;
            }
        }

        private void OnEnable()
        {
            // If the GameTestRunner is located in the same Scene, it has to wait for loading of all other components
            // The reason for moving him back to the main Sene without using Test Framework was to stop disabling him
            _shouldRun = true;
        }

        private void Update()
        {
            if (!_shouldRun)
            {
                return;
            }
            _shouldRun = false;

            // This is temporarily a duplicate assignment hotfix, as a prevention to not to have default Log Level
            Log.Level = GameManager.Instance.LogLevel;

            // Runs the tests for all Players
            RunTests();
        }

        public void RunTests()
        {
            Passed = false;
            Failed = false;
            Log.d("*** " + typeof(GameTestRunner).Name + ".RunTests called ***");

            // Test for each Player
            for (var playerIndex = 0; playerIndex < GameManager.Instance.Players.Length; playerIndex++)
            {
                var player = GameManager.Instance.Players[playerIndex];

                // Player state initialization
                Player.CurrentPlayer = player;
                Log.d(this, "Switching tested Player to " + player.name);

                // Meta-tests
                if (!WasStarted)
                {
                    SelfTests();
                }

                // Tests of all Players
                switch (playerIndex)
                {
                    // GUI player with tests in the default file location and an access to the HotSwap
                    case 0:
                        try
                        {
                            if (!WasStarted || GameManager.Instance.HotSwap)
                            {
                                // Requires Players to prove their Tutorial level again, before executing their code
                                // This also disregards the influence of self-tests on the tutorial
                                Tutorial.Instance.TutorialResetIfPlayer();
                                RunPlayerTest();
                            }
                        }
                        catch (Exception e)
                        {
                            player.Lost(e);
                        }
                        break;
                    default:
                        try
                        {
                            if (!WasStarted)
                            {
                                RunEnemyPlayerExampleTest();
                            }
                        }
                        catch (Exception e)
                        {
                            player.Lost(e);
                        }
                        break;
                }
            }
            WasStarted = true;

            // Time-out after 10 minutes
            StartCoroutine(RunAfterSeconds(600, () =>
            {
                Log.i("----------------------------------");
                Log.i(">>    End of Game, stalemate    <<");
                Log.i("----------------------------------");
                IntegrationTest.Fail();
                Failed = true;
                foreach (var player in GameManager.Instance.Players)
                {
                    player.Lost(Player.LoseReason.TimeoutStalemate);
                }
            }));
        }

        public void SelfTests()
        {
            // GameManager starting Unit creation
            Assert.AreEqual(1, BuildingHelper.GetMyBuildings<IBuilding>().Length);
            Assert.AreEqual(1, BuildingHelper.GetMyBuildings<BaseCenter>().Length);
            var units = GameManager.Instance.StartingWorkers;
            Assert.AreEqual(units, UnitHelper.GetMyUnits<IUnit>().Length);
            Assert.AreEqual(units, UnitHelper.GetMyUnits<Worker>().Length);

            // Position Grid coordinate calculation Unit Test
            var center = PlaceType.MyBase.Center;
            Assert.AreEqual(center.PointLocation, new Position(center.X, center.Y).PointLocation);
        }

        public void DebugTestUnitQueue()
        {
            var workers = UnitHelper.GetMyUnitsAsList<Worker>();
            workers[0]
                .MoveTo(PlaceType.MyBase.Left)
                .After(new CustomFunction(() =>
                    workers[0].CreateBuilding(BuildingType.BaseCenter, PlaceType.MyBase.Left)))
                .After(new Move(PlaceType.MyBase.Right))
                .After(new GatherResource(ResourceHelper.GetNearestMineralTo(workers[0])))
                .After(new CustomFunction(() =>
                    workers[0].CreateBuilding(BuildingType.BaseCenter, PlaceType.MyBase.Right)));

            var enemyBaseMovement = new Move(PlaceType.EnemyBase.Center);
            workers[1]
                .MoveTo(PlaceType.EnemyBase.UnderRampRight)
                .After(enemyBaseMovement);
            workers[2].ActionQueue = new UnitActionQueue(enemyBaseMovement);

            GameEvent.Register(GameEventType.EnemyBuildingsOnSight, args =>
            {
                if (args.EnemyBuildings[0] is BaseCenter & args.MyUnit != null)
                {
                    Log.d("Found enemy base, returning");
                    args.MyUnit.MoveTo(PlaceType.MyBase.Center);
                }
                else
                {
                    Log.d("Found building " + args.EnemyBuildings[0].GetType());
                }
            });
        }

        public void DebugTestBuilding()
        {
            var workers = UnitHelper.GetMyUnitsAsList<Worker>();
            workers[0].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Right);
            StartCoroutine(RunAfterSeconds(1, () =>
            {
                for (var index = 1; index < workers.Count; index++)
                {
                    workers[index].SendGather(ResourceHelper.GetNearestMineralTo(workers[index]));
                }
            }));

            GameEvent.Register(GameEventType.MineralsChanged, args =>
            {
                Log.i(this, args.Minerals + " minerals");
                var building = workers[0].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Left);
                workers[1].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Back);
                workers[2].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Front);
                foreach (var worker in workers)
                {
                    worker.FinishBuiding(building);
                }
                args.ThisGameEvent.UnregisterEvent();
            });
        }

        public void RunPlayerTest()
        {
            Log.d(">>  Starting a Planning Phase.  <<");

            var end = new Action(() => { Log.d(">>   End of a Planning Phase.   <<"); });

            if (_playerBotData == null)
            {
                _playerBotData = ScriptableObject.CreateInstance<MyBotData>();
            }

            var battleStage = new Action(() =>
            {
                Log.d(this, "Starting Battle Stage");
                if (_playerBattleTest == null)
                {
                    _playerBattleTest = ScriptableObject.CreateInstance<BattleTest>();
                }
                _playerBattleTest.MyBotData = _playerBotData;
                _playerBattleTest.BattleStage(end);
            });

            var buildingStage = new Action(() =>
            {
                Log.d(this, "Starting Building Stage");
                if (_playerBuildingTest == null)
                {
                    _playerBuildingTest = ScriptableObject.CreateInstance<BuildingTest>();
                }
                _playerBuildingTest.MyBotData = _playerBotData;
                _playerBuildingTest.BuildingStage(battleStage);
            });

            var economyStage = new Action(() =>
            {
                if (_playerEconomyTest == null)
                {
                    _playerEconomyTest = ScriptableObject.CreateInstance<EconomyTest>();
                }
                _playerEconomyTest.MyBotData = _playerBotData;
                _playerEconomyTest.EconomyStage(buildingStage);
            });

            economyStage();
        }

        public void RunEnemyPlayerExampleTest()
        {
            Log.d(">>  Starting a Planning Phase.  <<");

            var end = new Action(() => { Log.d(">>   End of a Planning Phase.   <<"); });

            if (_enemyPlayerBotData == null)
            {
                _enemyPlayerBotData = ScriptableObject.CreateInstance<TestExamples.MyBotData>();
            }

            var battleStage = new Action(() =>
            {
                Log.d(this, "Starting Battle Stage");
                if (_enemyPlayerBattleTest == null)
                {
                    _enemyPlayerBattleTest = ScriptableObject.CreateInstance<TestExamples.BattleTest>();
                }
                _enemyPlayerBattleTest.MyBotData = _enemyPlayerBotData;
                _enemyPlayerBattleTest.BattleStage(end);
            });

            var buildingStage = new Action(() =>
            {
                Log.d(this, "Starting Building Stage");
                if (_enemyPlayerBuildingTest == null)
                {
                    _enemyPlayerBuildingTest = ScriptableObject.CreateInstance<TestExamples.BuildingTest>();
                }
                _enemyPlayerBuildingTest.MyBotData = _enemyPlayerBotData;
                _enemyPlayerBuildingTest.BuildingStage(battleStage);
            });

            var economyStage = new Action(() =>
            {
                Log.d(this, "Starting Economy Stage");
                if (_enemyPlayerEconomyTest == null)
                {
                    _enemyPlayerEconomyTest = ScriptableObject.CreateInstance<TestExamples.EconomyTest>();
                }
                _enemyPlayerEconomyTest.MyBotData = _enemyPlayerBotData;
                _enemyPlayerEconomyTest.EconomyStage(buildingStage);
            });

            economyStage();
        }
    }
}
