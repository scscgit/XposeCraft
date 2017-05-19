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
        public new List<UnitType> SupportsUnitProduction
        {
            get { return base.SupportsUnitProduction; }
        }

        public new bool CanNowProduceUnit(UnitType unitType)
        {
            return base.CanNowProduceUnit(unitType);
        }

        public new void ProduceUnit(UnitType unitType)
        {
            base.ProduceUnit(unitType);
        }

        public new int QueuedUnits
        {
            get { return base.QueuedUnits; }
        }

        public new int QueueLimit
        {
            get { return base.QueueLimit; }
        }
    }
}
