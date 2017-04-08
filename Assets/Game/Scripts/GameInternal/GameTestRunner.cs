using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
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

        protected string SuccessString(bool success)
        {
            return success ? "finished successfully" : "failed";
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

            var gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            Player.CurrentPlayer = gameManager.Players[0];

            Assert.AreEqual(1, BuildingHelper.GetBuildings<IBuilding>().Length);
            Assert.AreEqual(1, BuildingHelper.GetBuildings<BaseCenter>().Length);
            var units = gameManager.StartingWorkers;
            Assert.AreEqual(units, UnitHelper.GetUnits<IUnit>().Length);
            Assert.AreEqual(units, UnitHelper.GetUnits<Worker>().Length);

            var workers = UnitHelper.GetUnitsAsList<Worker>();

            StartCoroutine(RunAfterSeconds(1, () =>
            {
                var minerals = ResourceHelper.GetResources<Mineral>();
                for (var index = 1; index < workers.Count; index++)
                {
                    workers[index].SendGather(minerals[index]);
                }
            }));

            Event.Register(EventType.MineralsChanged, args =>
            {
                Log.i(this, args.Minerals + " minerals");
                var building = workers[0].CreateBuilding(BuildingType.NubianArmory, PlaceType.MyBase.Left);
                foreach (var worker in workers)
                {
                    worker.FinishBuiding(building);
                }
            });

            RunPlayerTests();

            StartCoroutine(RunAfterSeconds(29, () =>
            {
                Log.i("----------------------------------");
                Log.i(">>         End of Game.         <<");
                Log.i("----------------------------------");
                IntegrationTest.Pass();
            }));
        }

        public void RunPlayerTests()
        {
            Log.i("----------------------------------");
            Log.i(">>  Starting a Planning Phase.  <<");
            Log.i("----------------------------------");

            var end = new Action(() =>
            {
                Log.i("----------------------------------");
                Log.i(">>   End of a Planning Phase.   <<");
                Log.i("----------------------------------");
            });

            var battleStage = new Action(() =>
            {
                Log.i(this, "Starting Battle Stage");
                var battleTest = new BattleTest();
                battleTest.BattleStage(end);
                Log.i(battleTest, "End of Battle Stage");
            });

            var buildingStage = new Action(() =>
            {
                Log.i(this, "Starting Building Stage");
                var buildingTest = new BuildingTest();
                buildingTest.BuildingStage(battleStage);
                Log.i(buildingTest, "End of Building Stage");
            });

            var economyStage = new Action(() =>
            {
                Log.i(this, "Starting Economy Stage");
                var economyTest = new EconomyTest();
                economyTest.EconomyStage(buildingStage);
                Log.i(economyTest, "End of Economy Stage");
            });

            economyStage();
        }
    }
}
