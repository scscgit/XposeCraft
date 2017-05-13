using System.Collections.Generic;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    public class NubianArmory : Building, IUnitProduction
    {
        /// <inheritdoc cref="Building.CanProduceUnits"/>
        public new List<UnitType> CanProduceUnits
        {
            get { return base.CanProduceUnits; }
        }

        /// <inheritdoc cref="Building.ProduceUnit"/>
        public new bool ProduceUnit(UnitType unitType)
        {
            return base.ProduceUnit(unitType);
        }

        /// <inheritdoc cref="Building.QueuedUnits"/>
        public new int QueuedUnits
        {
            get { return base.QueuedUnits; }
        }

        /// <inheritdoc cref="Building.QueueLimit"/>
        public new int QueueLimit
        {
            get { return base.QueueLimit; }
        }
    }
}
