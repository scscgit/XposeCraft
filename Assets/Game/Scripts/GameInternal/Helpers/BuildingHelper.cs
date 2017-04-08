using System;
using System.ComponentModel;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game.Actors.Buildings;
using XposeCraft.Game.Enums;
using Building = XposeCraft.Core.Required.Building;

namespace XposeCraft.GameInternal.Helpers
{
    public class BuildingHelper
    {
        public static Type DetermineBuildingType(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.BaseCenter:
                    return typeof(BaseCenter);
                case BuildingType.NubianArmory:
                    return typeof(NubianArmory);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Finds a Building representation of a chosen BuildingType within current Player's Faction.
        /// </summary>
        /// <param name="buildingType">Type that the Building has to represent.</param>
        /// <param name="unitController">If not null, will assert that the unit can build the Building.</param>
        /// <returns>Building representation.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Building FindBuildingInFaction(BuildingType buildingType, UnitController unitController)
        {
            for (var index = 0; index < Player.CurrentPlayer.Faction.BuildingList.Length; index++)
            {
                var building = Player.CurrentPlayer.Faction.BuildingList[index];
                if (!IsBuildingOfType(building.obj.name, buildingType))
                {
                    continue;
                }
                if (unitController != null && !unitController.build.build[index].canBuild)
                {
                    throw new InvalidOperationException(
                        "Building of the chosen building type cannot be built by this Unit");
                }
                return building;
            }
            throw new InvalidOperationException(
                "Building of the chosen building type is not available in your Faction");
        }

        public static bool IsBuildingOfType(string buildingName, BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.BaseCenter:
                    return buildingName.Equals("HelidroneStation");
                case BuildingType.NubianArmory:
                    return buildingName.Equals("NubianArmory");
                default:
                    return false;
            }
        }
    }
}
