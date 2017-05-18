using System;
using System.Collections.Generic;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    /// <summary>
    /// A base building that creates new Workers and receives collected Resources.
    /// </summary>
    public class BaseCenter : Building, IUnitProduction
    {
        public Position SpawnPosition
        {
            get { throw new NotImplementedException(); }
        }

        public List<UnitType> SupportsUnitProduction
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanNowProduceUnit(UnitType unitType)
        {
            throw new NotImplementedException();
        }

        public void ProduceUnit(UnitType unitType)
        {
            throw new NotImplementedException();
        }

        public int QueuedUnits
        {
            get { throw new NotImplementedException(); }
        }

        public int QueueLimit
        {
            get { throw new NotImplementedException(); }
        }
    }
}
