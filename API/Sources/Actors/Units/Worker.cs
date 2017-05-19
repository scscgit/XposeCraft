using System;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Control;
using XposeCraft.Game.Enums;

namespace XposeCraft.Game.Actors.Units
{
    /// <summary>
    /// Unit that can gather various Resources and build various Buildings.
    /// </summary>
    public class Worker : Unit
    {
        /// <summary>
        /// Currently gathered resource, null if the Worker is not currently gathering anything.
        /// </summary>
        public IResource Gathering { get; internal set; }

        /// <summary>
        /// Send the Worker to gather a resource.
        /// <see cref="XposeCraft.Game.Helpers.ResourceHelper"/> provides various methods to find some.
        /// </summary>
        /// <param name="resource">Resource to be gathered.</param>
        public UnitActionQueue SendGather(IResource resource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends the Worker to Create a Building of a specific type.
        /// </summary>
        /// <param name="buildingType">Type of the Building to be created.</param>
        /// <param name="position">Position where the Building will be placed.
        /// Use Position.IsValidPlacement to check if the position can be used.</param>
        /// <returns>Instance of the building in a progress state.</returns>
        public IBuilding CreateBuilding(BuildingType buildingType, Position position)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send the Worker to finish the construction of an existing building.
        /// </summary>
        /// <param name="building">Building to have its construction finished.</param>
        /// <returns>True if the request was processed as valid.</returns>
        public bool FinishBuiding(IBuilding building)
        {
            throw new NotImplementedException();
        }
    }
}
