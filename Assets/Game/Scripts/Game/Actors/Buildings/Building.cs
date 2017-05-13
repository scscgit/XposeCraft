using System;
using System.Collections.Generic;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Required;
using XposeCraft.GameInternal;
using XposeCraft.GameInternal.Helpers;
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

        public float ConstructionProgress
        {
            get { return BuildingController.progressCur / BuildingController.progressReq; }
        }

        protected List<UnitType> CanProduceUnits
        {
            get
            {
                var canProduceUnits = new List<UnitType>();
                foreach (var value in Enum.GetValues(typeof(UnitType)))
                {
                    var unitType = (UnitType) value;
                    try
                    {
                        UnitHelper.FindUnitIndexInFaction(unitType, BuildingController);
                        canProduceUnits.Add(unitType);
                    }
                    catch (Exception)
                    {
                        // Unsupported Unit
                    }
                }
                return canProduceUnits;
            }
        }

        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        internal void AttackedByUnit(UnitController attackerUnit)
        {
            if (GameManager.Instance.Factions[BuildingController.FactionIndex]
                    .Relations[attackerUnit.FactionIndex]
                    .state != 2)
            {
                throw new Exception("The target Building is not enemy, so it cannot be attacked");
            }
            UnitSelection.SetTarget(new List<UnitController> {attackerUnit}, GameObject, GameObject.transform.position);
        }

        /// <summary>
        /// Internal method, do not use.
        /// </summary>
        internal void FinishBuildingByWorker(UnitController builderUnit)
        {
            if (GameManager.Instance.Factions[BuildingController.FactionIndex]
                    .Relations[builderUnit.FactionIndex]
                    .state == 2)
            {
                throw new Exception("The target Building belongs to an enemy, so a worker cannot be construct it");
            }
            UnitSelection.SetTarget(new List<UnitController> {builderUnit}, GameObject, GameObject.transform.position);
        }

        protected bool ProduceUnit(UnitType unitType)
        {
            return BuildingController.unitProduction.StartProduction(
                UnitHelper.FindUnitIndexInUnitProduction(unitType, BuildingController.unitProduction),
                GameManager.Instance.ResourceManagerFaction[BuildingController.FactionIndex],
                BuildingController.PlayerOwner);
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
            BuildingController.PlayerOwner = playerOwner;
            if (!GameObject.CompareTag("Building"))
            {
                throw new InvalidOperationException("Building Actor has invalid state, GameObject is missing tag");
            }
        }

        internal void Placed(BuildingController placedBuilding, Player playerOwner)
        {
            if (BuildingController == null)
            {
                throw new Exception("Cannot be placed by a null controller");
            }
            placedBuilding.PlayerOwner = playerOwner;
            GameObject = placedBuilding.gameObject;
            BuildingController = placedBuilding;
        }
    }
}
