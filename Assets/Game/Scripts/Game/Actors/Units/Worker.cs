using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Actors.Resources;
using XposeCraft.Game.Control;
using XposeCraft.Game.Control.GameActions;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;
using BuildingType = XposeCraft.Game.Enums.BuildingType;

namespace XposeCraft.Game.Actors.Units
{
    /// <summary>
    /// Can gather various resources and build various buildings.
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
            if (resource != null)
            {
                Tutorial.Instance.SendGather();
            }
            return ActionQueue = new UnitActionQueue(new GatherResource(resource));
        }

        // TODO:
        // 1. send worker to the position near building, queued event when arrival
        // 2. start construction object, queued event when finished
        // 2.5. if interrupted, worker can repeat step 1 and continue on 2 without creating a new object
        // 3. finished event, return to gather
        public IBuilding CreateBuilding(BuildingType buildingType, Position position)
        {
            StopGathering();
            var player = Player.CurrentPlayer;
            var actor = Create<Building>(
                BuildingHelper.DetermineBuildingType(buildingType),
                BuildingPlacement.PlaceProgressBuilding(
                    BuildingHelper.FindBuildingInFaction(buildingType, UnitController),
                    new List<UnitController> {UnitController},
                    UnitController.FactionIndex,
                    position,
                    Quaternion.identity,
                    GameManager.Instance.ResourceManagerFaction[UnitController.FactionIndex]
                ),
                player
            );
            // TODO: make sure the CurrentPlayer stays the same after the movement
            Tutorial.Instance.CreateBuilding();
            // TODO: asynchronous after the movement
            GameManager.Instance.FiredEvent(player, GameEventType.BuildingStartedConstruction, new Arguments
            {
                MyUnit = this,
                MyBuilding = actor
            });
            return actor;
        }

        /// <summary>
        /// Send the Worker to finish the construction of an existing building.
        /// </summary>
        /// <param name="building">Building to have its construction finished.</param>
        /// <returns>True if the request was processed as valid.</returns>
        public bool FinishBuiding(IBuilding building)
        {
            StopGathering();
            if (building.Finished)
            {
                return false;
            }
            ((Building) building).FinishBuildingByWorker(UnitController);
            return true;
        }

        private void StopGathering()
        {
            Gathering = null;
        }
    }
}
