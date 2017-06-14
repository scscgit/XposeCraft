using System;
using System.ComponentModel;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Core.Fog_Of_War;
using XposeCraft.Core.Required;
using XposeCraft.Game.Actors.Units;
using Object = UnityEngine.Object;
using Unit = XposeCraft.Core.Required.Unit;
using UnitType = XposeCraft.Game.Enums.UnitType;

namespace XposeCraft.GameInternal.Helpers
{
    public class UnitHelper
    {
        public static Type DetermineUnitType(UnitType unitType)
        {
            switch (unitType)
            {
                case UnitType.Worker:
                    return typeof(Worker);
                case UnitType.DonkeyGun:
                    return typeof(DonkeyGun);
                case UnitType.WraithRaider:
                    return typeof(WraithRaider);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public static UnitType DetermineFactionUnitType(Unit unit)
        {
            switch (unit.obj.name)
            {
                case "Goblin":
                    return UnitType.Worker;
                case "DonkeyGun":
                    return UnitType.DonkeyGun;
                case "WraithRaiderStarship":
                    return UnitType.WraithRaider;
                default:
                    throw new Exception("Cannot determine the Unit Type from the Faction Unit");
            }
        }

        /// <summary>
        /// Finds a Unit representation of a chosen UnitType within current Player's Faction.
        /// </summary>
        /// <param name="unitType">Type that the Unit has to represent.</param>
        /// <param name="buildingController">If not null, will assert that the building can produce the Unit.</param>
        /// <returns>Unit representation index in the Faction.</returns>
        /// <exception cref="InvalidOperationException">Unit of the chosen type isn't valid in the context.</exception>
        public static int FindUnitIndexInFaction(UnitType unitType, BuildingController buildingController)
        {
            for (var index = 0; index < Player.CurrentPlayer.Faction.UnitList.Length; index++)
            {
                var unit = Player.CurrentPlayer.Faction.UnitList[index];
                if (!IsUnitOfType(unit.obj.name, unitType))
                {
                    continue;
                }
                if (buildingController != null
                    && !buildingController.unitProduction.canProduce
                    && !buildingController.unitProduction.units[index].canProduce)
                {
                    throw new InvalidOperationException(
                        "Unit " + unitType + " cannot be produced by this Building");
                }
                return index;
            }
            throw new InvalidOperationException(
                "Unit " + unitType + " is not available in your Faction");
        }

        public static int FindUnitIndexInUnitProduction(UnitType unitType, SUnitBuilding unitProduction)
        {
            string typeName;
            switch (unitType)
            {
                case UnitType.Worker:
                    typeName = "Goblin";
                    break;
                case UnitType.DonkeyGun:
                    typeName = "DonkeyGun";
                    break;
                default:
                    throw new Exception("Unknown Unit Type name");
            }
            for (var unitIndex = 0; unitIndex < unitProduction.units.Length; unitIndex++)
            {
                if (typeName == unitProduction.units[unitIndex].customName)
                {
                    return unitIndex;
                }
            }
            throw new Exception(
                "Cannot determine " + unitType + "'s unit index for the Unit Production from its custom name");
        }

        public static UnitType PrefabToType(GameObject unitPrefab)
        {
            foreach (var value in (UnitType[]) Enum.GetValues(typeof(UnitType)))
            {
                if (IsUnitOfType(unitPrefab.name, value))
                {
                    return value;
                }
            }
            throw new InvalidOperationException("Unit prefab name isn't valid and doesn't represent a type");
        }

        private static bool IsUnitOfType(string unitName, UnitType unitType)
        {
            switch (unitType)
            {
                case UnitType.Worker:
                    return unitName.Equals("Goblin");
                case UnitType.DonkeyGun:
                    return unitName.Equals("DonkeyGun");
                default:
                    return false;
            }
        }

        public static GameObject InstantiateUnit(GameObject prefab, Vector3 location, int factionIndex)
        {
            var obj = Object.Instantiate(prefab, location, Quaternion.identity);
            var script = obj.GetComponent<UnitController>();
            ChangeUnitFaction(script, factionIndex);
            return obj;
        }

        private static void ChangeUnitFaction(UnitController unitController, int factionIndex)
        {
            // Changes the Faction to a correct one, removing previous vision registration initialized during start
            var signal = unitController.GetComponent<VisionSignal>();
            var receiver = unitController.GetComponent<VisionReceiver>();
            if (signal)
            {
                signal.OnDisable();
            }
            if (receiver)
            {
                receiver.OnDisable();
            }
            unitController.FactionIndex = factionIndex;
            if (signal)
            {
                signal.OnEnable();
            }
            if (receiver)
            {
                receiver.OnEnable();
            }
        }
    }
}
