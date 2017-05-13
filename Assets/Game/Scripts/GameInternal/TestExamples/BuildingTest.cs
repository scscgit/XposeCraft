using System;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;
using XposeCraft.Game.Helpers;

namespace XposeCraft.GameInternal.TestExamples
{
    /// <summary>
    /// Druha faza hry.
    ///
    /// Cielom je pomocou pracovnikov vytvarat nove budovy,
    /// ktore budu produkovat vojenske jednotky, alebo inak ich vylepsovanim rozsirovat pravdepodobnost vyhry.
    /// </summary>
    class BuildingTest
    {
        /// <summary>
        /// Finds a worker that is just gathering any materials, without any other task
        /// </summary>
        /// <returns>A bored worker</returns>
        private Worker FindWorkerThatGathers()
        {
            foreach (Worker worker in UnitHelper.GetMyUnits<Worker>())
            {
                if (worker.Gathering != null)
                {
                    return worker;
                }
            }
            // First one by default if all have work, a bad workaround
            return UnitHelper.GetMyUnits<Worker>()[0];
        }

        public void BuildingStage(Action startNextStage)
        {
            // A first building
            Event.Register(EventType.MineralsChanged, args =>
            {
                if (args.Minerals >= 100)
                {
                    var baseCenter = BuildingHelper.GetMyBuildings<BaseCenter>()[0];
                    var position = BuildingHelper.ClosestEmptySpaceTo(baseCenter, BuildingType.NubianArmory);
                    IBuilding building = FindWorkerThatGathers().CreateBuilding(BuildingType.NubianArmory, position);
                    FindWorkerThatGathers().FinishBuiding(building);

                    // We only need one army production building for now
                    args.ThisEvent.UnregisterEvent();
                }
            });

            // Worker will return to work afterwards
            Event.Register(EventType.BuildingCreated, args =>
            {
                if (args.MyBuilding is NubianArmory
                    &&
                    args.MyUnit is Worker)
                {
                    var worker = (Worker) args.MyUnit;
                    worker.SendGather(ResourceHelper.GetNearestMineralTo(worker));
                    BuildArmy(startNextStage);
                }
                args.ThisEvent.UnregisterEvent();
            });
        }

        void BuildArmy(Action startNextStage)
        {
            Event.Register(EventType.MineralsChanged, args =>
            {
                if (args.Minerals > 100)
                {
                    foreach (NubianArmory armory in BuildingHelper.GetMyBuildings<NubianArmory>())
                    {
                        if (armory.ProduceUnit(UnitType.DonkeyGun))
                        {
                            if (MyBot.Army++ >= 5)
                            {
                                args.ThisEvent.UnregisterEvent();
                                startNextStage();
                            }
                            break;
                        }
                    }
                }
            });
        }
    }
}
