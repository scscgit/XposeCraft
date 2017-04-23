using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources.Minerals;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;
using XposeCraft.Test;
using Event = XposeCraft.Game.Event;
using EventType = XposeCraft.Game.Enums.EventType;

namespace XposeCraft.GameInternal
{
    public class GameTestRunner : MonoBehaviour
    {
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
            Log.i("*** " + typeof(GameTestRunner).Name + ".RunTests called ***");

            // Test for each Player
            foreach (var player in GameManager.Instance.Players)
            {
                // Player state initialization
                Player.CurrentPlayer = player;

                // Meta-tests
                SelfTests();

                // Temporary Example Test
                ExampleTest();

                // Game bot run
                RunPlayerTests();
            }

            StartCoroutine(RunAfterSeconds(29, () =>
            {
                Log.i("----------------------------------");
                Log.i(">>         End of Game.         <<");
                Log.i("----------------------------------");
                IntegrationTest.Pass();
            }));
        }

        public void SelfTests()
        {
            // GameManager starting Unit creation
            Assert.AreEqual(1, BuildingHelper.GetBuildings<IBuilding>().Length);
            Assert.AreEqual(1, BuildingHelper.GetBuildings<BaseCenter>().Length);
            var units = GameManager.Instance.StartingWorkers;
            Assert.AreEqual(units, UnitHelper.GetUnits<IUnit>().Length);
            Assert.AreEqual(units, UnitHelper.GetUnits<Worker>().Length);

            // Position Grid coordinate calculation Unit Test
            var center = PlaceType.MyBase.Center;
            Assert.AreEqual(center.PointLocation, new Position(center.X, center.Y).PointLocation);
        }

        public void ExampleTest()
        {
            var workers = UnitHelper.GetUnitsAsList<Worker>();
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
