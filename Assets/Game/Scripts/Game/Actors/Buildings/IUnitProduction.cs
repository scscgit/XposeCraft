using System.Collections.Generic;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    public interface IUnitProduction
    {
        /// <summary>
        /// Units that the Building can produce, empty if not this is not production building.
        /// </summary>
        List<UnitType> CanProduceUnits { get; }

        /// <summary>
        /// Queues a Unit production.
        /// </summary>
        /// <param name="unitType">Unit to be produced.</param>
        /// <returns>True if the operation succeeded, false if there was a problem, e.g. a lack of resources.</returns>
        bool ProduceUnit(UnitType unitType);

        int QueuedUnits { get; }

        int QueueLimit { get; }
    }
}
