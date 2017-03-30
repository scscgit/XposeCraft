using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using XposeCraft.Game.Actors;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Helpers;

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

        public delegate void NextStageStarter();

        private void Start()
        {
            RunTests();
        }

        public void RunTests()
        {
            print("*** " + typeof(GameTestRunner).Name + ".RunTests called ***");

            var gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            var nubianFactoryMock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nubianFactoryMock.name = "NubianFactoryMock";
            gameManager.Players[0].Buildings.Add(Actor.Create<NubianArmory>(nubianFactoryMock));

            Player.CurrentPlayer = gameManager.Players[0];
            Assert.AreEqual(1, BuildingHelper.GetBuildings<IBuilding>().Length);

            IntegrationTest.Pass();

            /*
            // Creating Model
            Model.Instance.Buildings.Add(new BaseCenter(PlaceType.NearBase));
            Model.Instance.Units.Add(new Worker(PlaceType.NearBase));

            Log.i(null, "----------------------------------");
            Log.i(null, ">>  Starting a new Test Round.  <<");
            Log.i(null, "----------------------------------");

            var battleStage = new NextStageStarter(() =>
            {
                Log.i(this, "Starting Battle Stage");
                var battle = new BattleTest();
                bool result = Battle(battle, () => { });
                Log.i(battle, "End of Battle Stage: " + SuccessString(result));
            });

            var buildingStage = new NextStageStarter(() =>
            {
                Log.i(this, "Starting Building Stage");
                var building = new BuildingTest();
                bool result = Building(building, battleStage);
                Log.i(building, "End of Building Stage: " + SuccessString(result));
            });

            var economyStage = new NextStageStarter(() =>
            {
                Log.i(this, "Starting Economy Stage");
                var economy = new EconomyTest();
                bool result = Economy(economy, buildingStage);
                Log.i(economy, "End of Economy Stage: " + SuccessString(result));
            });

            economyStage();

            Log.i(null, "");
            Log.i(null, ">>   End of a Planning Phase.   <<");
            Log.i(null, "");

            var gameTimer = new GameTimer();
            gameTimer.RunGame(() =>
            {
                if (gameTimer.Cycle >= 500)
                {
                    return true;
                }
                return false;
            });

            Log.i(null, "");
            Log.i(null, ">>   End of Game.   <<");
            Log.i(null, "");
            */
        }

        /*
        bool Economy(EconomyTest economy, NextStageStarter startNextStage)
        {
            economy.EconomyStage(startNextStage);
            return true;
        }

        bool Building(BuildingTest building, NextStageStarter startNextStage)
        {
            building.BuildingStage(startNextStage);
            return true;
        }

        bool Battle(BattleTest battle, NextStageStarter startNextStage)
        {
            battle.BattleStage(startNextStage);
            return true;
        }
        */
    }
}