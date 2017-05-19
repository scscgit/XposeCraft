using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    /// <summary>
    /// <see cref="DonkeyGun"/> production Building, a first available figher factory.
    /// </summary>
    public class NubianArmory : Building, IUnitProduction
    {
        public List<UnitType> SupportsUnitProduction { get; }

        public bool CanNowProduceUnit(UnitType unitType)
        {
            throw new NotImplementedException();
        }

        public void ProduceUnit(UnitType unitType)
        {
            throw new NotImplementedException();
        }

        public int QueuedUnits { get; }

        public int QueueLimit { get; }
    }
}
