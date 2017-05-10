using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.GameInternal;
using BuildingType = XposeCraft.Core.Faction.Buildings.BuildingType;
using UnitType = XposeCraft.Game.Enums.UnitType;

namespace XposeCraft.Game.Actors.Buildings
{
    public abstract class Building : Actor, IBuilding
    {
        protected BuildingController BuildingController;

        public bool Finished
        {
            get { return BuildingController.buildingType == BuildingType.CompleteBuilding; }
        }

        public float Progress
        {
            get { return BuildingController.progressCur / BuildingController.progressReq; }
        }

        public void AttackedByUnit(UnitController attackerUnit)
        {
            if (GameManager.Instance.Factions[BuildingController.FactionIndex]
                    .Relations[attackerUnit.FactionIndex]
                    .state != 2)
            {
                throw new Exception("The target Building is not enemy, so it cannot be attacked");
            }
            UnitSelection.SetTarget(new List<UnitController> {attackerUnit}, GameObject, GameObject.transform.position);
        }

        public void FinishBuildingByWorker(UnitController builderUnit)
        {
            if (GameManager.Instance.Factions[BuildingController.FactionIndex]
                    .Relations[builderUnit.FactionIndex]
                    .state == 2)
            {
                throw new Exception("The target Building belongs to an enemy, so a worker cannot be construct it");
            }
            UnitSelection.SetTarget(new List<UnitController> {builderUnit}, GameObject, GameObject.transform.position);
        }

        protected bool CreateUnit(UnitType type)
        {
            // TODO: add to the queue, event when created
            return false;
        }

        protected int QueuedUnits
        {
            get { return BuildingController.unitProduction.jobsAmount; }
        }

        protected int QueueLimit
        {
            get { return BuildingController.unitProduction.maxAmount; }
        }

        protected override void Initialize(Player playerOwner)
        {
            base.Initialize(playerOwner);
            BuildingController = GameObject.GetComponent<BuildingController>();
            if (!GameObject.CompareTag("Building"))
            {
                throw new InvalidOperationException("Building Actor has invalid state, GameObject is missing tag");
            }
        }
    }
}
