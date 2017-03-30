using System;
using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.Game.Actors.Materials;
using XposeCraft.GameInternal;
using BuildingType = XposeCraft.Game.Enums.BuildingType;

namespace XposeCraft.Game.Actors.Units
{
    /// <summary>
    /// Can gather various materials (mainly minerals) and build various buildings.
    /// </summary>
    public class Worker : Unit
    {
        public IMaterial Gathering { get; private set; }

        private UnitController _unitController;

        protected override void Initialize()
        {
            base.Initialize();
            _unitController = GameObject.GetComponent<UnitController>();
        }

        public void SendGather(IMaterial material)
        {
            // TODO: override ReplaceActionQueue to set Gathering back to null; make Gathering an action
            Gathering = material;
        }

        // TODO:
        // 1. send worker to the position near building, queued event when arrival
        // 2. start construction object, queued event when finished
        // 2.5. if interrupted, worker can repeat step 1 and continue on 2 without creating a new object
        // 3. finished event, return to gather
        public void CreateBuilding(BuildingType buildingType, Position position)
        {
            BuildingPlacement.PlaceProgressBuilding(
                FindBuildingInFaction(buildingType),
                new List<UnitController> {_unitController},
                _unitController.FactionIndex,
                position,
                position.Location,
                Quaternion.identity,
                GameManager.Instance.UGrid.grids[GameManager.Instance.UGrid.index],
                GameManager.Instance.Fog,
                GameManager.Instance.ResourceManager
            );
        }

        private Building FindBuildingInFaction(BuildingType buildingType)
        {
            for (var index = 0; index < Player.CurrentPlayer.Faction.BuildingList.Length; index++)
            {
                var building = Player.CurrentPlayer.Faction.BuildingList[index];
                if (!building.obj.name.Equals(buildingType.ToString()))
                {
                    continue;
                }
                if (!_unitController.build.build[index].canBuild)
                {
                    throw new InvalidOperationException(
                        "Building of the chosen building type cannot be built by this Unit");
                }
                return building;
            }
            throw new InvalidOperationException(
                "Building of the chosen building type is not available in your Faction");
        }
    }
}
