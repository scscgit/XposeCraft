using System;
using System.ComponentModel;
using UnityEngine;
using XposeCraft.Core.Faction.Buildings;
using XposeCraft.Core.Faction.Units;
using XposeCraft.Game;
using XposeCraft.Game.Actors.Buildings;
using Building = XposeCraft.Core.Required.Building;
using BuildingType = XposeCraft.Game.Enums.BuildingType;
using Object = UnityEngine.Object;

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

        public static GameObject InstantiateProgressBuilding(
            Building building, GameObject buildingPrefab, int factionIndex, Position position, Quaternion rotation)
        {
            // Placement validation is done redundantly twice, because this can be called directly too
            var location = PositionHelper.PositionToLocation(position);
            try
            {
                CheckValidPlacement(building, position, location, false);
            }
            catch (Exception)
            {
                // Visualizing the error placement
                building.ClosePoints(GameManager.Instance.Grid, position.PointLocation);
                throw;
            }
            building.ClosePoints(GameManager.Instance.Grid, position.PointLocation);
            GameObject buildingObject = Object.Instantiate(buildingPrefab, location, rotation);
            BuildingController script = buildingObject.GetComponent<BuildingController>();
            script.building = building;
            script.loc = position.PointLocation;
            script.FactionIndex = factionIndex;
            return buildingObject;
        }

        public static void CheckValidPlacement(
            Building building, Position position, Vector3 location, bool invalidUnderFog)
        {
            if (!IsValidPlacement(building, position, location, invalidUnderFog))
            {
                throw new InvalidOperationException(building + "'s building placement location is invalid");
            }
        }

        public static bool IsValidPlacement(
            Building building, Position position, Vector3 location, bool invalidUnderFog)
        {
            return building.CheckPoints(GameManager.Instance.Grid, position.PointLocation)
                   && (!invalidUnderFog || GameManager.Instance.Fog.CheckLocation(location));
        }
    }
}
