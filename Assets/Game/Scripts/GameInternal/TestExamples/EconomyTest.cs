using System;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;

namespace XposeCraft.GameInternal.TestExamples
{
    /// <summary>
    /// Prva faza hry.
    ///
    /// Cielom je zbierat suroviny pomocou jednotiek pracovnikov
    /// a pri dostatocnom pocte surovin vytvarat dalsich pracovnikov na zrychlenie ekonomie.
    /// </summary>
    internal class EconomyTest : BotScript
    {
        public MyBotData MyBotData;
        private GameEvent _producingWorker;

        public void EconomyStage(Action startNextStage)
        {
            BotRunner.Tutorial = false;
            BotRunner.HotSwap = false;

            // Game started, the first worker will get to work
            Worker[] firstWorkers = UnitHelper.GetMyUnits<Worker>();
            foreach (Worker worker in firstWorkers)
            {
                Gather(worker);
            }

            EventForCreatingAnother();
            BattleTest.RegisterReceiveFire();
            startNextStage();
        }

        void EventForCreatingAnother()
        {
            GameEvent.Register(GameEventType.MineralsChanged, argsA =>
            {
                if (_producingWorker == null && argsA.Minerals >= 50)
                {
                    // After he collected minerals, another worker will be built
                    var baseCenter = BuildingHelper.GetMyBuildings<BaseCenter>()[0];
                    baseCenter.ProduceUnit(UnitType.Worker);

                    // After creating (it means after few seconds), he will need to go gather too
                    _producingWorker = GameEvent.Register(GameEventType.UnitProduced, argsB =>
                    {
                        if (argsB.MyUnit is Worker)
                        {
                            Worker worker = (Worker) argsB.MyUnit;
                            Gather(worker);
                            argsB.ThisGameEvent.UnregisterEvent();
                            _producingWorker = null;
                        }
                    });
                }

                // This event will work only while there are not enough workers.
                // After that, minerals will be left to go over 150.
                if (UnitHelper.GetMyUnits<Worker>().Length >= 7)
                {
                    argsA.ThisGameEvent.UnregisterEvent();
                }
            });
        }

        public static void Gather(Worker worker)
        {
            var resource = ResourceHelper.GetNearestMineralTo(worker);
            if (resource == null)
            {
                BotRunner.Log("Worker couldn't find another Resource");
                return;
            }
            if (CreateBaseIfNone(worker))
            {
                return;
            }
            worker.SendGather(resource).After(new CustomFunction(() => Gather(worker)));
        }

        private static bool CreateBaseIfNone(Worker worker)
        {
            // If all bases were destroyed, try to make a new one
            var bases = BuildingHelper.GetMyBuildings<BaseCenter>();
            if (bases.Length == 0)
            {
                worker.CreateBuilding(
                    BuildingType.BaseCenter,
                    BuildingHelper.ClosestEmptySpaceTo(
                        BuildingHelper.GetMyBuildings<IBuilding>()[0],
                        BuildingType.BaseCenter));
                return true;
            }
            if (!bases[0].Finished)
            {
                worker.FinishBuiding(bases[0]);
                return true;
            }
            return false;
        }
    }
}
