using System.Collections.Generic;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Buildings
{
    /// <summary>
    /// Adds ability to produce Units to a Building.
    /// </summary>
    public interface IUnitProduction
    {
        /// <summary>
        /// Units that the Building can produce if provided resources, empty if not this is not production building.
        /// </summary>
        List<UnitType> SupportsUnitProduction { get; }

        /// <summary>
        /// Checks if it can now produce a Unity of a chosen type, fulfilling all conditions like resources.
        /// </summary>
        /// <param name="unitType">Unit type to check if it would be successful to produce right now.</param>
        /// <returns>True if the Unit type can be produced right now.</returns>
        bool CanNowProduceUnit(UnitType unitType);

        /// <summary>
        /// Queues a Unit production.
        /// </summary>
        /// <param name="unitType">Unit to be produced.</param>
        /// <returns>True if the operation succeeded, false if there was a problem, e.g. a lack of resources.</returns>
        /// <exception cref="UnitProductionException">Unit could not be produced in this Building.</exception>
        /// <exception cref="NotEnoughResourcesException">Not enough resources to enqueue the production.</exception>
        void ProduceUnit(UnitType unitType);

        /// <summary>
        /// Number of currently queued Units that will be produced.
        /// </summary>
        int QueuedUnits { get; }

        /// <summary>
        /// Maximum number of Units that can be queued at once to be produced.
        /// </summary>
        int QueueLimit { get; }
    }
}
