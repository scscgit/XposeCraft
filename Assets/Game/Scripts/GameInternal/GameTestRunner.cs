using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources.Minerals;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;
using XposeCraft.Test;
using Event = XposeCraft.Game.Event;
using EventType = XposeCraft.Game.Enums.EventType;

namespace XposeCraft.GameInternal
{
    public class GameTestRunner : MonoBehaviour
    {
        public static bool Passed { get; set; }
        public static bool Failed { get; set; }

        protected void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        protected IEnumerator RunAfterSeconds(int seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
        }

        private void Start()
        {
            RunTests();
        }

        public void RunTests()
        {
            Passed = false;
            Failed = false;
            Log.i("*** " + typeof(GameTestRunner).Name + ".RunTests called ***");

            // Test for each Player
            for (var playerIndex = 0; playerIndex < GameManager.Instance.Players.Length; playerIndex++)
            {
                var player = GameManager.Instance.Players[playerIndex];

                // Player state initialization
                Player.CurrentPlayer = player;
                Log.i(this, "Switching tested Player to " + player.name);

                // Meta-tests
                SelfTests();

                // Temporary Example Test
                switch (playerIndex)
                {
                    case 0:
                        ExampleTestUnitQueue();
                        break;
                    default:
                        ExampleTestBuilding();
                        break;
                }

                // Game bot run
                RunPlayerTests();
            }

            StartCoroutine(RunAfterSeconds(29, () =>
            {
                Log.i("----------------------------------");
                Log.i(">>         End of Game.         <<");
                Log.i("----------------------------------");
                IntegrationTest.Fail();
                Failed = true;
                //IntegrationTest.Pass();
                //Passed = true;
            }));
        }

        private void Update()
        {
            // Quits the game (or the editor when running from batch) after the test ends; pauses when in Editor
            if (Failed)
            {
                Log.e("Integration test failed");
                if (SystemInfo.graphicsDeviceID == 0)
                {
                    //EditorApplication.Exit(1);
                    throw new Exception("Integration test failed");
                }
                else
                {
                    Application.Quit();
                    Failed = false;
                }
            }
            else if (Passed)
            {
                Log.i("Integration test passed");
                if (SystemInfo.graphicsDeviceID == 0)
                {
                    //EditorApplication.Exit(0);
                    Application.Quit();
                    throw new Exception("Could not quit the game");
                }
                else
                {
                    Application.Quit();
                    Passed = false;
                }
            }
        }

        public void SelfTests()
        {
            // GameManager starting Unit creation
            Assert.AreEqual(1, BuildingHelper.GetBuildings<IBuilding>().Length);
            Assert.AreEqual(1, BuildingHelper.GetBuildings<BaseCenter>().Length);
            var units = GameManager.Instance.StartingWorkers;
            Assert.AreEqual(units, UnitHelper.GetMyUnits<IUnit>().Length);
            Assert.AreEqual(units, UnitHelper.GetMyUnits<Worker>().Length);

            // Position Grid coordinate calculation Unit Test
            var center = PlaceType.MyBase.Center;
            Assert.AreEqual(center.PointLocation, new Position(center.X, center.Y).PointLocation);
        }

        public void ExampleTestUnitQueue()
        {
            var workers = UnitHelper.GetMyUnitsAsList<Worker>();
            workers[0]
                .MoveTo(PlaceType.MyBase.Left)
                .After(new CustomFunction(() =>
                    workers[0].CreateBuilding(BuildingType.BaseCenter, PlaceType.MyBase.Left)))
                .After(new Move(PlaceType.MyBase.Right))
                .After(new GatherResource(ResourceHelper.GetNearestResourceTo<Mineral>(workers[0])))
                .After(new CustomFunction(() =>
                    workers[0].CreateBuilding(BuildingType.BaseCenter, PlaceType.MyBase.Right)));

            var enemyBaseMovement = new Move(PlaceType.EnemyBase.Center);
            workers[1]
                .MoveTo(PlaceType.EnemyBase.UnderRamp)
                .After(enemyBaseMovement);
            workers[2].ActionQueue = new UnitActionQueue(enemyBaseMovement);

            Event.Register(EventType.EnemyBuildingsOnSight, args =>
            {
                if (args.EnemyBuildings[0] is BaseCenter)
                {
                    Log.i("Found enemy base, returning");
                    workers[1].MoveTo(PlaceType.MyBase.Center);
                    workers[2].MoveTo(PlaceType.MyBase.Center);
                }
                else
                {
                    Log.i("Found building " + args.EnemyBuildings[0].GetType());
                }
            });
        }

        public void ExampleTestBuilding()
        {
            var workers = UnitHelper.GetMyUnitsAsList<Worker>();
            workers[0].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Right);
            StartCoroutine(RunAfterSeconds(1, () =>
            {
                for (var index = 1; index < workers.Count; index++)
                {
                    workers[index].SendGather(ResourceHelper.GetNearestResourceTo<Mineral>(workers[index]));
                }
            }));

            Event.Register(EventType.MineralsChanged, args =>
            {
                Log.i(this, args.Minerals + " minerals");
                var building = workers[0].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Left);
                workers[1].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Back);
                workers[2].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Front);
                foreach (var worker in workers)
                {
                    worker.FinishBuiding(building);
                }
                args.ThisEvent.UnregisterEvent();
            });
        }

        public void RunPlayerTests()
        {
            Log.i(">>  Starting a Planning Phase.  <<");

            var end = new Action(() => { Log.i(">>   End of a Planning Phase.   <<"); });

            var battleStage = new Action(() =>
            {
                Log.i(this, "Starting Battle Stage");
                var battleTest = new BattleTest();
                battleTest.BattleStage(end);
            });

            var buildingStage = new Action(() =>
            {
                Log.i(this, "Starting Building Stage");
                var buildingTest = new BuildingTest();
                buildingTest.BuildingStage(battleStage);
            });

            var economyStage = new Action(() =>
            {
                Log.i(this, "Starting Economy Stage");
                var economyTest = new EconomyTest();
                economyTest.EconomyStage(buildingStage);
            });

            economyStage();
        }
    }
}
