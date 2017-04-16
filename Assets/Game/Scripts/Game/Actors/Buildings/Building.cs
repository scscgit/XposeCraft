using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using BuildingType = XposeCraft.Core.Faction.Buildings.BuildingType;
using UnitType = XposeCraft.Game.Enums.UnitType;

namespace XposeCraft.Game.Actors.Buildings
{
    public abstract class Building : Actor, IBuilding
    {
        protected BuildingController BuildingController;

        public bool Finished
        {
            get { return BuildingController.buildingType.Equals(BuildingType.CompleteBuilding); }
        }

        public float Progress
        {
            get { return BuildingController.progressCur / BuildingController.progressReq; }
        }

        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        public void FinishBuildingByWorker(List<UnitController> builderUnits)
        {
            UnitSelection.SetTarget(builderUnits, GameObject, GameObject.transform.position);
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

        protected override void Initialize()
        {
            base.Initialize();
            BuildingController = GameObject.GetComponent<BuildingController>();
            if (!GameObject.CompareTag("Building"))
            {
                throw new InvalidOperationException("Building Actor has invalid state, GameObject is missing tag");
            }
        }
    }
}
