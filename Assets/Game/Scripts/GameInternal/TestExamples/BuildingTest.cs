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
    internal class BuildingTest : BotScript
    {
        public MyBotData MyBotData;
        private GameEvent _producingDonkeyGun;

        /// <summary>
        /// Finds a worker that is just gathering any materials, without any other task
        /// </summary>
        /// <returns>A bored worker</returns>
        public static Worker FindWorkerThatGathers()
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
            GameEvent.Register(GameEventType.MineralsChanged, args => BuildNubianArmory(args));

            GameEvent.Register(GameEventType.BuildingCreated, args =>
            {
                // A second building
                if (BuildNubianArmory(args))
                {
                    args.ThisGameEvent.UnregisterEvent();

                    // A third building
                    GameEvent.Register(GameEventType.BuildingCreated, argsThird =>
                    {
                        // A second building
                        if (BuildNubianArmory(argsThird))
                        {
                            argsThird.ThisGameEvent.UnregisterEvent();
                        }
                    });
                }
            });

            GameEvent.Register(GameEventType.BuildingCreated, args =>
            {
                // Worker will return to work afterwards
                if (args.MyBuilding is NubianArmory
                    &&
                    args.MyUnit is Worker)
                {
                    var worker = (Worker) args.MyUnit;
                    EconomyTest.Gather(worker);
                    BuildArmy(startNextStage);
                }
                args.ThisGameEvent.UnregisterEvent();
            });
        }

        private bool BuildNubianArmory(Arguments args)
        {
            if (args.Minerals < 100)
            {
                return false;
            }
            var baseCenter = BuildingHelper.GetMyBuildings<BaseCenter>()[0];
            var position = BuildingHelper.ClosestEmptySpaceTo(baseCenter, BuildingType.NubianArmory);
            if (position == null)
            {
                return false;
            }
            IBuilding building = FindWorkerThatGathers().CreateBuilding(BuildingType.NubianArmory, position);
            FindWorkerThatGathers().FinishBuiding(building);

            // We only need one army production building for now
            args.ThisGameEvent.UnregisterEvent();
            return true;
        }

        void BuildArmy(Action startNextStage)
        {
            GameEvent.Register(GameEventType.MineralsChanged, argsMinerals =>
            {
                // First wave starts at 5 units
                if (!MyBotData.Rushed && MyBotData.Army >= 5)
                {
                    startNextStage();
                    MyBotData.Rushed = true;
                }
                // Production ends after 20
                else if (MyBotData.Army >= 20)
                {
                    argsMinerals.ThisGameEvent.UnregisterEvent();
                    return;
                }
                if (_producingDonkeyGun != null || argsMinerals.Minerals <= 100)
                {
                    return;
                }
                foreach (NubianArmory armory in BuildingHelper.GetMyBuildings<NubianArmory>())
                {
                    if (armory.QueuedUnits >= 2)
                    {
                        continue;
                    }
                    if (armory.CanNowProduceUnit(UnitType.DonkeyGun))
                    {
                        armory.ProduceUnit(UnitType.DonkeyGun);
                        // Additional mineral sink: producing one more expensive Unit if too rich
                        if (armory.CanNowProduceUnit(UnitType.WraithRaider))
                        {
                            armory.ProduceUnit(UnitType.WraithRaider);
                        }
                        _producingDonkeyGun = GameEvent.Register(GameEventType.UnitProduced, argsUnit =>
                        {
                            if (argsUnit.MyUnit is DonkeyGun)
                            {
                                MyBotData.Army++;
                                BotRunner.Log("My army contains " + MyBotData.Army + " battle units");
                                argsUnit.ThisGameEvent.UnregisterEvent();
                                _producingDonkeyGun = null;
                            }
                        });
                    }
                }
            });
        }
    }
}
