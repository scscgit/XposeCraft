using System;
using System.Collections.Generic;
using UnityEngine;
using XposeCraft.Game.Enums;
using XposeCraft.GameInternal;

namespace XposeCraft.Game.Actors.Buildings
{
    /// <summary>
    /// A base building that creates new workers and receives collected materials.
    /// </summary>
    public class BaseCenter : Building, IUnitProduction
    {
        protected const string SpawnPositionName = "Front";

        public Position SpawnPosition
        {
            get
            {
                foreach (var component in GameObject.GetComponentsInChildren<Transform>())
                {
                    if (component.name == SpawnPositionName)
                    {
                        return new Position(GameManager.Instance.UGrid.DetermineLocation(component.position));
                    }
                }
                throw new InvalidOperationException(
                    "Building " + this + " does not contain spawn position " + SpawnPositionName);
            }
        }

        public new List<UnitType> CanProduceUnits
        {
            get { return base.CanProduceUnits; }
        }

        public new bool ProduceUnit(UnitType unitType)
        {
            return base.ProduceUnit(unitType);
        }

        public new int QueuedUnits
        {
            get { return base.QueuedUnits; }
        }

        public new int QueueLimit
        {
            get { return base.QueueLimit; }
        }
    }
}
