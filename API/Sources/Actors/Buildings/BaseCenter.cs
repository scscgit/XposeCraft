using System;
using System.Collections.Generic;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Actors.Units;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    /// <summary>
    /// A base building that creates new <see cref="Worker"/>s and receives collected <see cref="IResource"/>s.
    /// </summary>
    public class BaseCenter : Building, IUnitProduction
    {
        public Position SpawnPosition { get; }

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
